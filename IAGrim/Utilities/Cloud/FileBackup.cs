using EvilsoftCommons.Cloud;
using IAGrim.Backup.FileWriter;
using IAGrim.Database.Interfaces;
using log4net;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using IAGrim.Settings;

namespace IAGrim.Utilities.Cloud {

    internal class FileBackup : ICloudBackup {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileBackup));
        private Stopwatch? _timer;
        private readonly SettingsService _settingsService;
        private readonly IPlayerItemDao _playerItemDao;

        private static readonly string[] AcceptedFileFormats = new[] {
            ".gdc",
            ".gdd",
            ".fow",
            ".dat",
            ".bin",
            ".gst",
            ".gsh"
        };

        public FileBackup(IPlayerItemDao playerItemDao, SettingsService settingsService) {
            this._playerItemDao = playerItemDao;
            this._settingsService = settingsService;
        }

        // Anything larger than this in a save folder is not a save file.
        private const long MaxFileSizeBytes = 1024 * 1024;

        private static bool IsAcceptedFileFormat(string s) {
            return AcceptedFileFormats.Contains(Path.GetExtension(s)) && !s.Contains("(");
        }

        /// <summary>
        /// The path to store a file under inside the archive. Zip entry names are
        /// specified to use forward slashes, and normalising here also keeps the
        /// backup layout identical between Windows and Linux.
        /// </summary>
        private static string RelativeEntryName(string root, string file) {
            return Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
        }

        public void Update() {
            if (_timer == null) {
                _timer = new Stopwatch();
                _timer.Start();
                Backup(false);
            } else if (_timer.ElapsedMilliseconds > 1000 * 60 * 30) {
                _timer.Restart();
                Backup(false);
            }
        }

        public bool Backup(bool forced) {
            try {
                List<string> paths = new List<string>();

                // God knows what the user has inputted here... lets err on the safe side.
                try {
                    string customPath = _settingsService.GetLocal().BackupCustomLocation;
                    if (_settingsService.GetLocal().BackupCustom && !string.IsNullOrEmpty(customPath)) {
                        if (!Directory.Exists(customPath))
                            Directory.CreateDirectory(customPath);

                        if (Directory.Exists(customPath)) {
                            paths.Add(customPath);
                        }
                    }
                } catch (Exception ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                }

                foreach (string path in paths) {
                    Backup(Path.Combine(path, "EvilSoft", "IAGD"), forced);
                }

                // Do a mandatory backup to appdata
                Backup(GlobalPaths.BackupLocation, false);
            } catch (UnauthorizedAccessException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            } catch (IOException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            } catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Every character in the "main" directory that looks backup-worthy.
        /// </summary>
        public static List<string> ListCharacters() {
            List<string> result = new List<string>();

            string characterFolder = Path.Combine(GlobalPaths.SavePath, "main");
            foreach (var character in Directory.GetDirectories(characterFolder)) {
                var f = Path.Combine(character, "player.gdc");
                if (!File.Exists(f))
                    continue;

                // Less than 4KB? Probably corrupted
                if (new FileInfo(f).Length < 4 * 1024)
                    continue;

                result.Add(Path.GetFileName(character));
            }

            return result;
        }

        public static bool StashFilesExist() {
            return StashFiles().Count > 0;
        }

        public static bool MyDocumentsGrimDawnExists() {
            string characterFolder = Path.Combine(GlobalPaths.SavePath, "main");
            return Directory.Exists(characterFolder);
        }

        private static List<string> StashFiles() {
            return new[] {"transfer.gst", "transfer.gsh"}
                .Select(f => Path.Combine(GlobalPaths.SavePath, f))
                .Where(File.Exists)
                .ToList();
        }

        /// <summary>
        /// A digest of everything that would go into this character's backup.
        /// Timestamps are deliberately not part of it: Grim Dawn rewrites save files
        /// on events that do not change their contents, and the whole point is to
        /// avoid re-uploading a character we have already backed up.
        /// </summary>
        public static string ComputeCharacterHash(string character) {
            var folder = Path.Combine(GlobalPaths.SavePath, "main", character);
            return ComputeHash(BackupCandidates(folder, SearchOption.AllDirectories), GlobalPaths.SavePath);
        }

        public static string ComputeStashHash() {
            return ComputeHash(BackupCandidates(GlobalPaths.SavePath, SearchOption.TopDirectoryOnly), GlobalPaths.SavePath);
        }

        /// <summary>
        /// The files under root which are eligible for backup, in the same order regardless
        /// of what order the filesystem hands them out.
        /// </summary>
        private static List<string> BackupCandidates(string root, SearchOption searchOption) {
            return Directory.GetFiles(root, "*.*", searchOption)
                .Where(IsAcceptedFileFormat)
                .Where(f => new FileInfo(f).Length <= MaxFileSizeBytes)
                .OrderBy(f => RelativeEntryName(root, f), StringComparer.Ordinal)
                .ToList();
        }

        private static string ComputeHash(List<string> files, string root) {
            using var sha = SHA256.Create();
            using var stream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write);

            foreach (var f in files) {
                // Name and length are part of the digest, so that renaming or truncating
                // a file registers as a change even if no file contents differ.
                var header = Encoding.UTF8.GetBytes($"{RelativeEntryName(root, f)}:{new FileInfo(f).Length}\n");
                stream.Write(header, 0, header.Length);

                using var file = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                file.CopyTo(stream);
            }

            stream.FlushFinalBlock();
            return Convert.ToHexString(sha.Hash!);
        }

        /// <summary>
        /// Creates a backup of a single character in the "main" directory (eg not mod)
        /// </summary>
        /// <param name="target">Target zip file (will be overwritten if exists)</param>
        /// <param name="character">Character name (with leading _ if applicable)</param>
        public static void BackupCharacter(string target, string character) {
            var folder = Path.Combine(GlobalPaths.SavePath, "main", character);
            Logger.Info($"Backing up character {character}..");

            // Stored relative to the save folder rather than the character folder, so the
            // archive can be unpacked straight into "My Games/Grim Dawn/Save".
            CreateArchive(target, BackupCandidates(folder, SearchOption.AllDirectories), GlobalPaths.SavePath,
                $"This backup of {character} was created at {DateTime.Now:G}.");
        }

        public static void BackupCommon(string target) {
            Logger.Info($"Backing up transfer files etc..");

            CreateArchive(target, BackupCandidates(GlobalPaths.SavePath, SearchOption.TopDirectoryOnly), GlobalPaths.SavePath,
                $"This backup of your stash files was created at {DateTime.Now:G}.");
        }

        /// <summary>
        /// Writes files to a new zip at target, replacing any existing archive.
        /// </summary>
        /// <remarks>
        /// ZipArchiveMode.Create opens with FileMode.CreateNew, which throws if the file
        /// is already there. Since targets are named per weekday they are always there
        /// the second time round, so deleting first is not optional.
        /// </remarks>
        private static void CreateArchive(string target, List<string> files, string root, string comment) {
            var destination = Path.GetDirectoryName(target);
            if (destination == null)
                return;
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            if (File.Exists(target)) {
                Logger.Info($"The file {target} already exists, deleting to create a new backup");
                File.Delete(target);
            }

            using var zip = ZipFile.Open(target, ZipArchiveMode.Create);
            foreach (var f in files) {
                zip.CreateEntryFromFile(f, RelativeEntryName(root, f));
            }

            zip.Comment = comment;
        }

        private void Backup(string destination, bool forced) {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);


#if DEBUG
            var suffix = "_DEBUG";
#else
            var suffix = string.Empty;
#endif
            string target = Path.Combine(destination, $"{DateTime.Now.DayOfWeek}{suffix}.zip");

            // If the file already exists and is newer than 3 days ('not written today'), just skip it.
            if (File.Exists(target) && !forced) {
                DateTime lastModified = File.GetLastWriteTime(target);
                if ((DateTime.Now - lastModified).TotalDays < 3)
                    return;
            }

            var items = _playerItemDao.ListAll();
            if (items.Count == 0) {
                Logger.Warn("No items found, skipping backup to avoid overwriting existing good backups.");
                return;
            }

            if (File.Exists(target)) {
                File.Delete(target);
            }

            using var file = new TempFile();
            using var zip = ZipFile.Open(target, ZipArchiveMode.Create);
            Logger.Info("Backing up characters..");
                    
            foreach (var f in BackupCandidates(GlobalPaths.SavePath, SearchOption.AllDirectories)) {
                zip.CreateEntryFromFile(f, RelativeEntryName(GlobalPaths.SavePath, f));
            }

            Logger.Info("Backing up items..");


            var exporter = new IAFileExporter(file.filename);
            exporter.Write(items);

            zip.CreateEntryFromFile(file.filename, "export.ias");

            string helpfile = Path.Combine("Resources", "YES THIS FILE IS SUPPOSED TO BE SMALL.txt");
            if (File.Exists(helpfile))
                zip.CreateEntryFromFile(helpfile, "YES THIS FILE IS SUPPOSED TO BE SMALL.txt");

            zip.Comment = string.Format("This backup was created at {0}.", System.DateTime.Now.ToString("G"));

            Logger.Info("Created a new backup of the database");
        }
    }
}

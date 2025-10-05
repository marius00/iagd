﻿using EvilsoftCommons.Cloud;
using IAGrim.Backup.FileWriter;
using IAGrim.Database.Interfaces;
using log4net;
using System.Diagnostics;
using System.IO.Compression;
using IAGrim.Settings;

namespace IAGrim.Utilities.Cloud {

    internal class FileBackup : ICloudBackup {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileBackup));
        private Stopwatch _timer;
        private readonly CloudWatcher _provider = new CloudWatcher();
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

        private static bool IsAcceptedFileFormat(string s) {
            return AcceptedFileFormats.Contains(Path.GetExtension(s)) && !s.Contains("(");
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
                
                if (_settingsService.GetLocal().BackupDropbox && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.DROPBOX).Location);
                
                if (_settingsService.GetLocal().BackupGoogle && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE).Location);
                
                if (_settingsService.GetLocal().BackupOnedrive && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.ONEDRIVE).Location);
                
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

        public static List<string> ListCharactersNewerThan(DateTime dt) {
            List<string> result = new List<string>();

            string characterFolder = Path.Combine(GlobalPaths.SavePath, "main");
            var characters = Directory.GetDirectories(characterFolder);
            foreach (var character in characters) {
                var f = Path.Combine(character, "player.gdc");
                if (!File.Exists(f))
                    continue;

                if (File.GetLastWriteTimeUtc(f) <= dt)
                    continue;

                // Less than 4KB? Probably corrupted
                if (new FileInfo(f).Length < 4 * 1024)
                    continue;

                result.Add(Path.GetFileName(character));
                
            }

            return result;
        }

        public static bool IsStashFilesNewerThan(DateTime dt) {
            string gameSaves = Path.Combine(GlobalPaths.SavePath, "Save");
            foreach (var file in new string[] {"transfer.gst", "transfer.gsh"}) {
                var filename = Path.Combine(gameSaves, file);
                if (File.Exists(filename)) {
                    if (File.GetLastWriteTimeUtc(filename) > dt) {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool MyDocumentsGrimDawnExists() {
            string characterFolder = Path.Combine(GlobalPaths.SavePath, "main");
            return Directory.Exists(characterFolder);
        }

        public static DateTime GetHighestCharacterTimestamp() {
            string characterFolder = Path.Combine(GlobalPaths.SavePath, "main");
            var characters = Directory.GetDirectories(characterFolder).ToList();
            foreach (var f in new[] {"transfer.gst", "transfer.gsh"}) {
                var filename = Path.Combine(GlobalPaths.SavePath, f);
                if (File.Exists(filename)) {
                    characters.Add(filename);
                }
            }

            if (characters.Count == 0)
                return default(DateTime);


            return characters
                .Select(File.GetLastWriteTimeUtc)
                .Max();
        }

        /// <summary>
        /// Creates a backup of a single character in the "main" directory (eg not mod)
        /// </summary>
        /// <param name="target">Target zip file (will be overwritten if exists)</param>
        /// <param name="character">Character name (with leading _ if applicable)</param>
        public static void BackupCharacter(string target, string character) {
            var destination = Path.GetDirectoryName(target);
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);


            string gameSaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save");

            string[] files = Directory.GetFiles(Path.Combine(gameSaves, "main", character), "*.*", SearchOption.AllDirectories);


            if (File.Exists(target)) {
                Logger.Info($"The file {target} already exists, deleting to create a new backup");
                File.Delete(target);
            }

            using var zip = ZipFile.Open(target, ZipArchiveMode.Create);
            Logger.Info($"Backing up character {character}..");

            foreach (var f in files) {
                if (!IsAcceptedFileFormat(f)) {
                    Logger.Debug($"Ignoring file {f}, invalid file format.");
                    continue;
                }

                // Max 1MB
                if (new FileInfo(f).Length > 1024 * 1024) {
                    Logger.Debug($"Ignoring file {f}, size exceeds 1MB");
                    continue;
                }

                var relativePath = f.Replace(gameSaves, "").Replace(Path.GetFileName(f), "");

                if (relativePath.StartsWith("\\")) {
                    relativePath = relativePath.Substring(1);
                }
                zip.CreateEntryFromFile(f, relativePath +"/"+ f);
            }

            zip.Comment = $"This backup of {character} was created at {DateTime.Now:G}.";
        }

        public static void BackupCommon(string target) {
            var destination = Path.GetDirectoryName(target);
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);


            string[] files = Directory.GetFiles(GlobalPaths.SavePath, "*.*", SearchOption.TopDirectoryOnly);

            using var zip = ZipFile.Open(target, ZipArchiveMode.Create);
            Logger.Info($"Backing up transfer files etc..");

            foreach (var f in files) {
                if (!IsAcceptedFileFormat(f)) {
                    Logger.Debug($"Ignoring file {f}, invalid file format.");
                    continue;
                }

                // Max 1MB
                if (new FileInfo(f).Length > 1024 * 1024) {
                    Logger.Debug($"Ignoring file {f}, size exceeds 1MB");
                    continue;
                }

                zip.CreateEntryFromFile(f, Path.GetFileName(f));
            }

            zip.Comment = $"This backup of your stash files was created at {DateTime.Now:G}.";

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
                    
            string[] files = Directory.GetFiles(GlobalPaths.SavePath, "*.*", SearchOption.AllDirectories);
            foreach (var f in files) {
                if (!IsAcceptedFileFormat(f)) {
                    Logger.Debug($"Ignoring file {f}, invalid file format.");
                    continue;
                }

                // Max 1MB
                if (new FileInfo(f).Length > 1024 * 1024) {
                    Logger.Debug($"Ignoring file {f}, size exceeds 1MB");
                    continue;
                }


                var relativePath = f.Replace(GlobalPaths.SavePath, "").Replace(Path.GetFileName(f), "");
                if (relativePath.StartsWith("\\")) {
                    relativePath = relativePath.Substring(1);
                }
                zip.CreateEntryFromFile(f, relativePath + Path.GetFileName(f));
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
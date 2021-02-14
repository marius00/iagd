using EvilsoftCommons.Cloud;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.FileWriter;
using IAGrim.Database.Interfaces;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            ".cpn",
            ".gst",
            ".gsh"
        };

        public FileBackup(IPlayerItemDao playerItemDao, SettingsService settingsService) {
            this._playerItemDao = playerItemDao;
            this._settingsService = settingsService;
        }

        private static bool IsAcceptedFileFormat(string s) {
            return AcceptedFileFormats.Contains(Path.GetExtension(s));
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
                ExceptionReporter.ReportException(ex, "Creating a backup");
                return false;
            }

            return true;
        }

        public static List<string> ListCharactersNewerThan(DateTime dt) {
            List<string> result = new List<string>();

            string characterFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save", "main");
            var characters = Directory.GetDirectories(characterFolder);
            foreach (var character in characters) {
                var f = Path.Combine(character, "player.gdc");
                if (!File.Exists(f))
                    continue;

                if (File.GetLastWriteTimeUtc(f) > dt) {
                    result.Add(Path.GetFileName(character));
                }
            }

            return result;
        }

        public static DateTime GetHighestCharacterTimestamp() {
            string characterFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save", "main");
            var characters = Directory.GetDirectories(characterFolder);
            return characters.Select(File.GetLastWriteTimeUtc).Max();
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

            using (ZipFile zip = new ZipFile { UseZip64WhenSaving = Zip64Option.AsNecessary }) {
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
                    zip.AddFile(f, relativePath);
                }

                zip.Comment = $"This backup of {character} was created at {DateTime.Now:G}.";

                try {
                    zip.Save(target);
                }
                catch (UnauthorizedAccessException) {
                    Logger.WarnFormat("Access denied writing backup to \"{0}\"", target);
                    throw;
                }
            }
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

            using (var file = new TempFile()) {
                using (ZipFile zip = new ZipFile { UseZip64WhenSaving = Zip64Option.AsNecessary }) {
                    Logger.Info("Backing up characters..");
                    string gameSaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save");
                    string[] files = Directory.GetFiles(gameSaves, "*.*", SearchOption.AllDirectories);
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
                        zip.AddFile(f, relativePath);
                    }

                    Logger.Info("Backing up items..");


                    var exporter = new IAFileExporter(file.filename);
                    exporter.Write(_playerItemDao.ListAll());

                    zip.AddFile(file.filename).FileName = "export.ias";

                    string helpfile = Path.Combine("Resources", "YES THIS FILE IS SUPPOSED TO BE SMALL.txt");
                    if (File.Exists(helpfile))
                        zip.AddFile(helpfile, "");

                    zip.Comment = string.Format("This backup was created at {0}.", System.DateTime.Now.ToString("G"));

                    try {
                        zip.Save(target);
                    }
                    catch (UnauthorizedAccessException) {
                        Logger.WarnFormat("Access denied writing backup to \"{0}\"", target);
                        throw;
                    }



                    Logger.Info("Created a new backup of the database");
                } //
            }
        }
    }
}
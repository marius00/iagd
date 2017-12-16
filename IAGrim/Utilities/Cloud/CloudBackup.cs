using EvilsoftCommons.Cloud;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.FileWriter;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using Ionic.Zip;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IAGrim.Utilities.Cloud {

    internal class CloudBackup : ICloudBackup {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CloudBackup));
        private Stopwatch _timer;
        private CloudWatcher _provider = new CloudWatcher();
        private readonly IPlayerItemDao _playerItemDao;

        public CloudBackup(IPlayerItemDao playerItemDao) {
            this._playerItemDao = playerItemDao;
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
                if ((bool)Properties.Settings.Default.BackupDropbox && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.DROPBOX).Location);

                if ((bool)Properties.Settings.Default.BackupGoogle && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE).Location);

                if ((bool)Properties.Settings.Default.BackupOnedrive && _provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE))
                    paths.Add(_provider.Providers.First(m => m.Provider == CloudProviderEnum.ONEDRIVE).Location);

                // God knows what the user has inputted here... lets err on the safe side.
                try {
                    string customPath = Properties.Settings.Default.BackupCustomLocation.ToString();
                    if ((bool)Properties.Settings.Default.BackupCustom && !string.IsNullOrEmpty(customPath)) {
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
        

        private void Backup(string destination, bool forced) {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);


#if DEBUG
            var suffix = "_DEBUG";
#else
            var suffix = string.Empty;
#endif
            string target = Path.Combine(destination, string.Format("{0}{1}.zip", DateTime.Now.DayOfWeek, suffix));

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
                    if (Directory.Exists(gameSaves)) {
                        zip.AddDirectory(gameSaves, "Save");
                    }

                    Logger.Info("Backing up items..");


                    var exporter = new IAFileExporter(file.filename);
                    exporter.Write(_playerItemDao.ListAll());

                    zip.AddFile(file.filename).FileName = "export.ias";

                    /*
                    try {
                        // TODO: This is now redundant, but leaving it in here "for safety" until the IAS format has proven itself.
                        zip.AddDirectory(GlobalPaths.UserdataFolder, "IAGD");
                    }
                    catch (Exception ex) {
                        Logger.Warn(ex.Message);
                        Logger.Warn(ex.StackTrace);
                        ExceptionReporter.ReportException(ex);
                    }*/


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
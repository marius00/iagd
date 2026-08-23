using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    internal static class GlobalPaths {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GlobalPaths));
        


        private static string LocalAppdata {
            get {
                string? appdata = System.Environment.GetEnvironmentVariable("LocalAppData");
                if (string.IsNullOrEmpty(appdata))
                    return Path.Combine(System.Environment.GetEnvironmentVariable("AppData") ?? string.Empty, "..", "local");
                else
                    return appdata;
            }
        }


        public static string ItemsHtmlFile => Path.Combine(StorageFolder, "index.html");

        public static string BackupLocation {
            get {
                string path = Path.Combine(CoreFolder, "backup");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocation {
            get {
                string path = Path.Combine(CoreFolder, "itemqueue");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvReplicaWriteLocation {
            get {
                string path = Path.Combine(CoreFolder, "replica", "from_ia");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvReplicaReadLocation {
            get {
                string path = Path.Combine(CoreFolder, "replica", "to_ia");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvReplicaDumpLocation {
            get {
                string path = Path.Combine(CoreFolder, "replica", "to_ia", "deleted");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocationIngoing {
            get {
                string path = Path.Combine(CsvLocation, "ingoing");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocationOutgoing {
            get {
                string path = Path.Combine(CsvLocation, "outgoing");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocationIngoingDeleted {
            get {
                string path = Path.Combine(CsvLocation, "ingoing", "deleted");
                Directory.CreateDirectory(path);
                return path;
            }
        }
        

        public static string DebugLocation{
            get {
                string path = Path.Combine(CoreFolder, "debug");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocationOutgoingDeleted {
            get {
                string path = Path.Combine(CsvLocation, "deleted");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CharacterBackupLocation {
            get {
                string path = Path.Combine(BackupLocation, "characters");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string EdgeCacheLocation{
            get {
                string path = Path.Combine(CoreFolder, "edge");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string LinuxHack {
            get {
                string path = Path.Combine(CoreFolder, "linuxhack");
                Directory.CreateDirectory(path);
                return path;
            }
        }



        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken
        );

        public static string? DownloadsFolder {
            get {
                Guid DownloadsFolderGuid = new Guid("{374DE290-123F-4565-9164-39C4925E467B}");
                try {
                    return SHGetKnownFolderPath(DownloadsFolderGuid, 0, IntPtr.Zero);
                }
                catch (Exception ex) {
                    Logger.Warn(ex);
                    return null;
                }
            }
        }

        public static string SavePath {
            get {
                var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save");
                Directory.CreateDirectory(p);
                return p;
            }
        }


        public static string UserdataFolder {
            get {
                string path = Path.Combine(CoreFolder, "data");
                Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string StorageFolder {
            get {
                string
                    path = Path.Combine(CoreFolder, "storage")
                        .Replace("#",
                            ""); // Some brilliant people have hashtags in their windows usernames..  That works poorly when opening HTML files with a # in the path.
                Directory.CreateDirectory(path);

                return path;
            }
        }

#if DEBUG
        public static string SettingsFile => Path.Combine(CoreFolder, "settings-debug.json").Replace("#", "");
#else
        public static string SettingsFile => Path.Combine(CoreFolder, "settings.json").Replace("#", "");
#endif

        public static string CoreFolder {
            get {
                string path = Path.Combine(LocalAppdata, "EvilSoft", "IAGD");
                Directory.CreateDirectory(path);

                return path;
            }
        }


#if DEBUG
        public static string USERDATA_FILE => "userdata-test.db";
#else
        public static string USERDATA_FILE => "userdata.db";
#endif
    }
}
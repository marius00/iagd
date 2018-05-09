using IAGrim.Parsers.Arz;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    internal static class GlobalPaths {
        private static HashSet<string> ParsedFiles = new HashSet<string>();
        private static List<GDTransferFile> TransferFilesCache = new List<GDTransferFile>();


        public static string LocalAppdata {
            get {
                String appdata = System.Environment.GetEnvironmentVariable("LocalAppData");
                if (string.IsNullOrEmpty(appdata))
                    return Path.Combine(System.Environment.GetEnvironmentVariable("AppData"), "..", "local");
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


        /// <summary>
        /// Return the formula files for GD
        /// Does not account for mods
        /// </summary>
        public static string[] FormulasFiles {
            get {
                string documents = SavePath;
                string file_n = Path.Combine(documents, "formulas.gst");
                string file_hc = Path.Combine(documents, "formulas.gsh");

                if (!File.Exists(file_n))
                    file_n = string.Empty;

                if (!File.Exists(file_hc))
                    file_hc = string.Empty;

                return new string[] { file_n, file_hc };
            }
        }



        public static bool IsHardcore(string filename) {
            return filename.EndsWith(".gsh");
        }

        public static string SavePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save");

        /// <summary>
        /// Map of [mod][transfer file]
        /// </summary>
        public static List<GDTransferFile> TransferFiles {
            get {
                string documents = SavePath;

                if (Directory.Exists(documents)) {

                    // Generate a list of the interesting files
                    List<string> files = new List<string>();
                    foreach (string filename in new string[] { "transfer.gst", "transfer.gsh" }) {
                        string vanilla = Path.Combine(documents, filename);
                        if (File.Exists(vanilla) && !ParsedFiles.Contains(vanilla)) {
                            files.Add(vanilla);
                        }


                        foreach (var possibleMod in Directory.GetDirectories(documents)) {
                            string mod = Path.Combine(possibleMod, filename);
                            if (File.Exists(mod) && !ParsedFiles.Contains(mod)) {
                                files.Add(mod);
                            }
                        }
                    }


                    foreach (string potential in files) {
                        string fn;
                        if (StashManager.TryGetModLabel(potential, out fn)) {
                            ParsedFiles.Add(potential);
                            var lastAccess = File.GetLastWriteTime(potential);
                            TransferFilesCache.Add(new GDTransferFile {
                                Filename = potential,
                                Mod = fn,
                                IsHardcore = IsHardcore(potential),
                                LastAccess = lastAccess
                            });
                        }

                    }
                }

                return new List<GDTransferFile>(TransferFilesCache);
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
                string path = Path.Combine(CoreFolder, "storage");
                Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string CoreFolder {
            get {
                string path = Path.Combine(LocalAppdata, "EvilSoft", "IAGD");
                Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string USERDATA_FILE_FIXED {
            get {
                return "userdata.db";
            }
        }

#if DEBUG
        public static string USERDATA_FILE {
            get {
                return "userdata-test.db";
            }
        }
#else
        public static string USERDATA_FILE {
            get {
                return USERDATA_FILE_FIXED;
            }
        }
#endif

    }
}

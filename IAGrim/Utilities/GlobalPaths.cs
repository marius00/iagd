using IAGrim.Parsers.Arz;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IAGrim.Utilities {
    internal static class GlobalPaths {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GlobalPaths));
        private static readonly HashSet<string> ParsedFiles = new HashSet<string>();
        private static readonly List<GDTransferFile> TransferFilesCache = new List<GDTransferFile>();


        private static string LocalAppdata {
            get {
                string appdata = System.Environment.GetEnvironmentVariable("LocalAppData");
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
                string formulasSoftcore = Path.Combine(documents, "formulas.gst");
                string formulasHardcore = Path.Combine(documents, "formulas.gsh");

                if (!File.Exists(formulasSoftcore))
                    formulasSoftcore = string.Empty;

                if (!File.Exists(formulasHardcore))
                    formulasHardcore = string.Empty;

                return new string[] { formulasSoftcore, formulasHardcore };
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
                        if (TransferStashService.TryGetModLabel(potential, out var mod)) {
                            ParsedFiles.Add(potential);
                            var lastAccess = File.GetLastWriteTime(potential);
                            TransferFilesCache.Add(new GDTransferFile {
                                Filename = potential,
                                Mod = mod,
                                IsHardcore = IsHardcore(potential),
                                LastAccess = lastAccess
                            });
                        }
                    }

                    if (TransferFilesCache.Count == 0) {
                        Logger.Warn($"No stash files detected in {documents}");
                    }
                }
                else {
                    Logger.Warn($"Could not locate the folder \"{documents}\"");
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


#if DEBUG
        public static string USERDATA_FILE => "userdata-test.db";
#else
        public static string USERDATA_FILE => "userdata.db";
#endif

    }
}

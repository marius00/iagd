using IAGrim.Parsers.Arz;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using static IAGrim.Utilities.HelperClasses.GDTransferFile;

namespace IAGrim.Utilities {
    internal static class GlobalPaths {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GlobalPaths));
        


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

        public static string CsvLocation {
            get {
                string path = Path.Combine(CoreFolder, "itemqueue");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string CsvLocationDeleted {
            get {
                string path = Path.Combine(CoreFolder, "itemqueue_deleted");
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

                return new string[] {formulasSoftcore, formulasHardcore};
            }
        }


        public static bool IsHardcore(string filename) {
            return filename.EndsWith(".gsh") || filename.EndsWith(".csh") || filename.EndsWith(".bsh");
        }

        /// <summary>
        /// Fetches the "downgrade type" of the transfer file.
        /// Transfer files ending with .cst/.csh have disabled the FG expansion.
        /// Transfer files ending with .bst/.bsh have disabled the AoM and FG expansions.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DowngradeType GetDowngradeType(string filename) {
            var withoutLastLetter = filename.Substring(0, filename.Length - 1);
            if (withoutLastLetter.EndsWith(".cs"))
                return DowngradeType.AoM;
            else if (withoutLastLetter.EndsWith(".bs"))
                return DowngradeType.NoExpansions;
            else
                return DowngradeType.None;
        }

        public static string SavePath {
            get {
                var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
                    "Grim Dawn", "Save");
                Directory.CreateDirectory(Path.Combine(p, "main"));
                return p;
            }
        }

        /// <summary>
        /// Map of [mod][transfer file]
        /// </summary>
        public static List<GDTransferFile> GetTransferFiles(bool includeDowngradeFiles) {
            var transferFilesCache = new List<GDTransferFile>();
            HashSet<string> parsedFiles = new HashSet<string>();
            string documents = SavePath;

            var transferFilenames = new string[] {
                "transfer.gst", // Softcore
                "transfer.gsh", // Hardcore
            };

            if (includeDowngradeFiles) {
                transferFilenames = new string[] {
                    "transfer.gst", // Softcore
                    "transfer.gsh", // Hardcore
                    "transfer.bst", // Softcore Vanilla FG/AOM disabled (Vanilla only, owns expansions)
                    "transfer.cst", // Softcore FG disabled (Vanilla+AOM) 
                    "transfer.csh", // Hardcore FG disabled (Vanilla+AOM) 
                    "transfer.bsh" // Hardcore FG/AOM disabled (Vanilla only, owns expansions)
                };
            }

            if (!Directory.Exists(documents)) {
                Logger.Warn($"Could not locate the folder \"{documents}\"");
                return transferFilesCache;
            }


            // Generate a list of the interesting files
            List<string> files = new List<string>();
            // transfer.bst / transfer.cst / transfer.csh
            foreach (string filename in transferFilenames) {
                string vanilla = Path.Combine(documents, filename);
                if (File.Exists(vanilla) && !parsedFiles.Contains(vanilla)) {
                    files.Add(vanilla);
                }


                foreach (var possibleMod in Directory.GetDirectories(documents)) {
                    string mod = Path.Combine(possibleMod, filename);
                    if (File.Exists(mod) && !parsedFiles.Contains(mod)) {
                        files.Add(mod);
                    }
                }
            }


            foreach (string potential in files) {
                if (TransferStashService.TryGetModLabel(potential, out var mod)) {
                    parsedFiles.Add(potential);
                    var lastAccess = File.GetLastWriteTime(potential);
                    transferFilesCache.Add(new GDTransferFile {
                        Filename = potential,
                        Mod = mod,
                        IsHardcore = IsHardcore(potential),
                        LastAccess = lastAccess,
                        Downgrade = GetDowngradeType(potential)
                    });
                }
            }

            if (transferFilesCache.Count == 0) {
                Logger.Warn($"No stash files detected in {documents}");
            }


            return transferFilesCache;
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
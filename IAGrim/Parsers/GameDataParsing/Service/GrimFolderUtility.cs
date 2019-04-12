using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Parsers.GameDataParsing.Service {
    static class GrimFolderUtility {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GrimFolderUtility));

        /// <summary>
        ///     Locate the ARZ file for a given mod or GD install
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static string FindArzFile(string parent) {
            if (File.Exists(parent) && parent.ToLower().EndsWith(".arz")) {
                return parent;
            }
            if (!Directory.Exists(Path.Combine(parent, "database"))) {
                Logger.WarnFormat("Could not find the \"database\" directory at \"{0}\"", parent);
            }
            else {
                foreach (var file in Directory.EnumerateFiles(Path.Combine(parent, "database"), "*.arz")) {
                    return file;
                }
            }

            return String.Empty;
        }


        /// <summary>
        /// Returns expansion folders from Grim Dawn, given a Grim Dawn install path.
        /// </summary>
        public static List<string> GetGrimExpansionFolders(string grimdawnLocation) {
            List<string> paths = new List<string>();

            bool AddIfExists(string p) {
                if (Directory.Exists(p)) {
                    paths.Add(p);
                    return true;
                }

                return false;
            }

            for (int i = 9; i >= 1; i--) {
                AddIfExists(Path.Combine(grimdawnLocation, $"gdx{i}"));
            }

            /*
             // No useful items, only writs. Not worth the insane parsing time.
            for (int i = 9; i >= 1; i--) {
                AddIfExists(Path.Combine(grimdawnLocation, $"survivalmode{i}"));
            }

            AddIfExists(Path.Combine(grimdawnLocation, "mods", "survivalmode"));
            */

            return paths;

        }

        /// <summary>
        ///     Locate a given Arc file for a given mod or GD install
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="arc"></param>
        /// <returns></returns>
        public static string FindArcFile(string parent, string arc) {
            if (File.Exists(Path.Combine(parent, "resources", arc)))
                return Path.Combine(parent, "resources", arc);
            return string.Empty;
        }
    }
}

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
        ///     Locate a given Arc file for a given mod or GD install
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="arc"></param>
        /// <returns></returns>
        public static string FindArcFile(string parent, string arc) {
            if (File.Exists(Path.Combine(parent, "resources", arc)))
                return Path.Combine(parent, "resources", arc);
            return String.Empty;
        }

        public static int CountExisting(params string[] files) {
            int count = 0;
            foreach (var file in files) {
                if (File.Exists(file)) {
                    count++;
                }
            }

            return count;
        }
    }
}

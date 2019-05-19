using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.UI.Model;
using IAGrim.Utilities;

namespace IAGrim.UI.Service {

    /// <summary>
    /// Perhaps more of a presenter
    /// </summary>
    class DatabaseModSelectionService {

        private string GetLabel(int expansionLevel) {
            var tagVanilla = RuntimeSettings.Language.GetTag("iatag_ui_vanilla");
            var tagVanillaXpac = RuntimeSettings.Language.GetTag("iatag_ui_vanilla_xpac");
            var tagForgottenGods = RuntimeSettings.Language.GetTag("iatag_ui_forgottengods");
            switch (expansionLevel) {
                case 0:
                    return tagVanilla;
                case 1:
                    return tagVanillaXpac;
                case 2:
                    return tagForgottenGods;
                default:
                    return $"{expansionLevel} Expansions"; // No translation, this is a worst case IA is outdated error.
            }
        }
        public List<ListViewItem> GetGrimDawnInstalls(IEnumerable<string> paths) {
            List<ListViewItem> entries = new List<ListViewItem>();

            foreach (var gdPath in paths) {
                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {

                    int expansionLevel = 0;
                    for (int i = 1; i <= 9; i++) {
                        expansionLevel = Directory.Exists(Path.Combine(gdPath, $"gdx{i}")) ? i : expansionLevel;
                    }
                    
                    ListViewItem vanilla = new ListViewItem(GetLabel(expansionLevel));
                    vanilla.SubItems.Add(gdPath);
                    vanilla.Tag = new ListViewEntry { Path = gdPath, IsVanilla = true };
                    entries.Add(vanilla);
                }

            }

            

            return entries;
        }

        public List<ListViewItem> GetInstalledMods(IEnumerable<string> paths) {
            List<ListViewItem> entries = new List<ListViewItem>();
            const string theCrucibleDlc = "survivalmode";
            var noModSelected = RuntimeSettings.Language.GetTag("iatag_ui_none");

            ListViewItem empty = new ListViewItem(noModSelected);
            empty.SubItems.Add("-");
            empty.Tag = null;
            entries.Add(empty);

            
            foreach (var gdPath in paths) {
                List<string> modpaths = new List<string>();
                modpaths.Add(Path.Combine(gdPath, "mods"));

                // Detect expansions and DLCs
                for (int i = 1; i <= 9; i++) {
                    string path = Path.Combine(gdPath, $"gdx{i}", "mods");
                    if (Directory.Exists(path)) {
                        modpaths.Add(path);
                    }
                }

                foreach (var modpath in modpaths) {
                    if (Directory.Exists(modpath)) {
                        foreach (var directory in Directory.EnumerateDirectories(modpath)) {
                            if (Directory.EnumerateFiles(directory, "*.arz", SearchOption.AllDirectories).Any()) {

                                // Just ignore the crucible
                                var modName = Path.GetFileName(directory);
                                if (modName == theCrucibleDlc) {
                                    continue;
                                }

                                ListViewItem mod = new ListViewItem(modName);
                                mod.SubItems.Add(directory);
                                mod.Tag = new ListViewEntry {Path = directory, IsVanilla = false};
                                entries.Add(mod);
                            }
                        }
                    }
                }
            }

            return entries;
        }
    }
}

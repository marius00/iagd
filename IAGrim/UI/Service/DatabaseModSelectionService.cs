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
        public List<ListViewItem> GetGrimDawnInstalls(IEnumerable<string> paths) {
            List<ListViewItem> entries = new List<ListViewItem>();

            var tagVanilla = GlobalSettings.Language.GetTag("iatag_ui_vanilla");
            var tagVanillaXpac = GlobalSettings.Language.GetTag("iatag_ui_vanilla_xpac");

            foreach (var gdPath in paths) {
                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {
                    bool hasExpansion = Directory.Exists(Path.Combine(gdPath, "gdx1"));
                    ListViewItem vanilla = new ListViewItem(hasExpansion ? tagVanillaXpac : tagVanilla);
                    vanilla.SubItems.Add(gdPath);
                    vanilla.Tag = new ListViewEntry { Path = gdPath, IsVanilla = true };
                    entries.Add(vanilla);
                }

            }


            return entries;
        }

        public List<ListViewItem> GetInstalledMods(IEnumerable<string> paths) {
            List<ListViewItem> entries = new List<ListViewItem>();
            const string TheCrucibleDLC = "survivalmode";
            var noModSelected = GlobalSettings.Language.GetTag("iatag_ui_none");

            ListViewItem empty = new ListViewItem(noModSelected);
            empty.SubItems.Add("-");
            empty.Tag = null;
            entries.Add(empty);

            foreach (var gdPath in paths) {
                foreach (var modpath in new[] {Path.Combine(gdPath, "mods"), Path.Combine(gdPath, "gdx1", "mods")}) {
                    if (Directory.Exists(modpath)) {
                        foreach (var directory in Directory.EnumerateDirectories(modpath)) {
                            if (Directory.EnumerateFiles(directory, "*.arz", SearchOption.AllDirectories).Any()) {

                                // Just ignore the crucible
                                var modName = Path.GetFileName(directory);
                                if (modName == TheCrucibleDLC) {
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

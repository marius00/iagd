using IAGrim.Database;
using log4net;
using StatTranslator;
using System.IO.Compression;
using System.Text;

namespace IAGrim.Parsers.Arz {
    public class LocalizationLoader {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LocalizationLoader));
        private IDictionary<string, string> _tagsItems = new Dictionary<string, string>();
        private IDictionary<string, string> _tagsIa = new Dictionary<string, string>();

        public ISet<ItemTag> GetItemTags() {
            ISet<ItemTag> stats = new HashSet<ItemTag>();
            foreach (var item in _tagsItems) {
                stats.Add(new ItemTag {
                    Name = item.Value,
                    Tag = item.Key
                });
            }

            return stats;
        }

        /// <summary>
        /// Recursively check each control for .Tag == "iatag_*"
        /// If the tag is located and also found in the provided language pack, the controls text is updated
        /// </summary>
        /// <param name="c"></param>
        /// <param name="lang"></param>
        public static void ApplyLanguage(Control.ControlCollection c, ILocalizedLanguage lang) {
            foreach (Control control in c) {
                ApplyLanguage(control, lang);
                ApplyLanguage(control.Controls, lang);
            }
        }

        public static void ApplyTooltipLanguage(ToolTip toolTip, Control.ControlCollection c, ILocalizedLanguage lang) {
            foreach (Control control in c) {
                ApplyLanguage(control, lang, toolTip);
                ApplyTooltipLanguage(toolTip, control.Controls, lang);
            }
        }

        private static void ApplyLanguage(Control control, ILocalizedLanguage lang, ToolTip toolTip = null) {
            var tag = control.Tag?.ToString();
            bool hasTag = tag?.StartsWith("iatag_") ?? false;

            if (hasTag) {
                // Some controls like TextBox will only have a _tooltip tag, don't insert the text into them.
                if (!tag.EndsWith("_tooltip")) {

                    var localizedTag = lang.GetTag(tag);
                    if (!string.IsNullOrEmpty(localizedTag)) {
                        control.Text = localizedTag;
                    } else if (lang.WarnIfMissing) {
                        Logger.WarnFormat("Could not find tag {0} in localization, defaulting to {0}={1}", tag, control.Text);
                    }
                }

                // Most controls only has a regular tag, but may contain a _tooltip tag too.
                if (toolTip != null && !string.IsNullOrEmpty(toolTip.GetToolTip(control))) {
                    var tooltipTagName = (tag + "_tooltip").Replace("_tooltip_tooltip", "_tooltip");

                    var localizedTooltipTag = lang.GetTag(tooltipTagName);
                    if (!string.IsNullOrEmpty(localizedTooltipTag)) {
                        toolTip.SetToolTip(control, localizedTooltipTag);
                    }
                    else if (lang.WarnIfMissing) {
                        Logger.WarnFormat("Could not find tag {0} in localization, defaulting to {0}={1}", tooltipTagName, toolTip.GetToolTip(control));
                    }
                }
            }
            else {
                // Listview column headers
                ListView lv = control as ListView;
                if (lv != null) {
                    foreach (ColumnHeader header in lv.Columns) {
                        ApplyLanguage(header, lang);
                    }
                }
            }
        }

        private static void ApplyLanguage(ColumnHeader control, ILocalizedLanguage lang) {
            var tag = control.Tag?.ToString();
            bool hasTag = tag?.StartsWith("iatag_") ?? false;
            if (hasTag) {
                var localizedTag = lang.GetTag(tag);
                if (!string.IsNullOrEmpty(localizedTag)) {
                    control.Text = localizedTag;
                }
                else if (lang.WarnIfMissing) {
                    Logger.WarnFormat("Could not find tag {0} in localization, defaulting to {0}={1}", tag,
                        control.Text);
                }
            }
        }

        public ILocalizedLanguage LoadLanguage(string filename, EnglishLanguage fallback) {
            if (_tagsItems == null || _tagsItems.Count == 0) {
                Load(filename);
            }

            var dataset = new Dictionary<string, string>(_tagsItems);
            if (_tagsIa != null) {
                foreach (var tag in _tagsIa) {
                    dataset[tag.Key] = tag.Value;
                }
            }

            var language = new ThirdPartyLanguage(dataset, fallback);
            return language;
        }

        private Dictionary<string, string> Parse(string data) {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in data.Split('\n')) {
                var sline = line.Split('=');
                if (sline.Length == 2) {
                    result[sline[0].Trim()] = sline[1].Replace("^k", "").TrimEnd('\r');
                }
            }

            return result;
        }

        private Dictionary<string, string> ReadFile(ZipArchive zip, string filename) {
            var f = zip.Entries.FirstOrDefault(m => m.Name == filename);
            using var handle = f.Open();
            var data = new StreamReader(handle, Encoding.UTF8).ReadToEnd();
            return Parse(data);
        }

        public bool CheckLanguage(string filename, out string author, out string language) {
            author = "Unknown";
            language = "Unknown";
            if (!File.Exists(filename))
                return false;
            
            try {
                using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);
                var parsed = ReadFile(zip, "language.def");
                if (parsed.ContainsKey("author"))
                    author = parsed["author"];
                if (parsed.ContainsKey("language"))
                    language = parsed["language"];

                return true;
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            }
        }

        static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y) {
            return x
                .Except(x.Join(y, z => z.Key, z => z.Key, (a, b) => a))
                .Concat(y)
                .ToDictionary(z => z.Key, z => z.Value);
        }


        public bool Load(string filename) {
            if (!File.Exists(filename))
                return false;

            try {
                using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);
                foreach (var itemsFile in zip.Entries.Where(m =>
                             m.Name.Contains("items")
                             || m.Name.Contains("skills")
                             || m.Name.Contains("endlessdungeon"))
                         ) {

                    using var handle = itemsFile.Open();
                    var data = new StreamReader(handle, Encoding.UTF8).ReadToEnd();
                    var tags = Parse(data);
                    _tagsItems = Merge(_tagsItems, tags);
                }

                var tagsIaFile = zip.Entries.FirstOrDefault(m => m.Name == "tags_ia.txt");
                if (tagsIaFile != null) {
                    _tagsIa = ReadFile(zip, "tags_ia.txt");
                }
                else {
                    Logger.WarnFormat("Could not locate tags_ia.txt in {0}, defaulting to english for missing tags.", filename);
                    _tagsIa = null;
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            }

            return true;
        }


        public static bool HasSupportedTranslations(IEnumerable<string> grimDawnInstallPaths) {
            foreach (var path in grimDawnInstallPaths) {
                if (Directory.Exists(Path.Combine(path, "localization"))) {
                    foreach (var file in Directory.EnumerateFiles(Path.Combine(path, "localization"), "*.zip")) {
                        if (IsFullySupportedTranslation(file)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsFullySupportedTranslation(string filename) {
            if (!File.Exists(filename)) {
                return false;
            }

            try {
                using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);
                return zip.Entries.FirstOrDefault(m => m.Name == "tags_ia.txt") != null;
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return false;
            }
        }
    }
}
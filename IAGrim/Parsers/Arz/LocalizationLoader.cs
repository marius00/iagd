using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IAGrim.Database;
using IAGrim.Parsers.GameDataParsing.Model;
using log4net;
using StatTranslator;

namespace IAGrim.Parsers.Arz {
    public class LocalizationLoader {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LocalizationLoader));
        private const int MaxMissingTagWarnings = 30;
        private static int _missingTagWarningCount = 0;        private IDictionary<string, string> _tagsItems = new Dictionary<string, string>();
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
                // TextBox and ComboBox tags are tooltip-only; skip them when not applying tooltips
                if (toolTip == null && (control is TextBox || control is ComboBox)) return;

                var localizedTag = lang.GetTag(tag);
                if (!string.IsNullOrEmpty(localizedTag)) {
                    if (toolTip != null) {
                        toolTip.SetToolTip(control, localizedTag);
                    }
                    else {
                        control.Text = localizedTag;
                    }
                }
                else if (lang.WarnIfMissing && _missingTagWarningCount < MaxMissingTagWarnings) {
                    _missingTagWarningCount++;
                    Logger.WarnFormat("Could not find tag {0} in localization, defaulting to {0}={1}", tag,
                        control.Text);
                    if (_missingTagWarningCount == MaxMissingTagWarnings) {
                        Logger.Warn("Suppressing further missing localization tag warnings...");
                    }
                }
            }
        }


        /// <summary>
        /// Load language using a language code and English fallback.
        /// Reads the bundled IA translation override file for the given language code.
        /// </summary>
        public ILocalizedLanguage LoadLanguage(string languageCode, EnglishLanguage fallback) {
            var iaTranslationFile = LanguageMapping.GetIaTranslationFile(languageCode);
            if (iaTranslationFile != null) {
                LoadIaTranslationFile(iaTranslationFile);
            } else {
                Logger.Info($"No bundled IA translation file found for language code '{languageCode}'");
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

        /// <summary>
        /// Load a plain .txt translation override file (replaces the old tags_ia.txt from zip).
        /// Format: key=value per line.
        /// </summary>
        public void LoadIaTranslationFile(string filePath) {
            if (!File.Exists(filePath)) {
                Logger.Warn($"IA translation file not found: {filePath}");
                return;
            }

            try {
                var data = File.ReadAllText(filePath, Encoding.UTF8);
                _tagsIa = Parse(data);
                Logger.Info($"Loaded {_tagsIa.Count} IA override tags from {filePath}");
            }
            catch (Exception ex) {
                Logger.Warn($"Error loading IA translation file {filePath}: {ex.Message}");
                _tagsIa = null;
            }
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

        static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y) {
            foreach (var entry in y) {
                x[entry.Key] = entry.Value;
            }

            return x;
        }

        /// <summary>
        /// Check if any GD install path has non-EN Text_XX.arc files available.
        /// </summary>
        public static bool HasSupportedTranslations(IEnumerable<string> grimDawnInstallPaths) {
            return LanguageMapping.GetAvailableLanguages(grimDawnInstallPaths)
                .Any(code => !code.Equals("EN", StringComparison.OrdinalIgnoreCase));
        }
    }
}
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace StatTranslator {
    public class ThirdPartyLanguage : ILocalizedLanguage {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThirdPartyLanguage));
        private const int MaxMissingTagWarnings = 30;
        private readonly Dictionary<string, string> _stats;
        private readonly ItemNameCombinator _itemCombinator;

        public bool WarnIfMissing => true;

        public ThirdPartyLanguage(Dictionary<string, string> dataset, EnglishLanguage fallback) {
            _stats = dataset;

            // Make sure the loaded language has all the necessary keys
            int missingCount = 0;
            foreach (var key in fallback.Serialize()) {
                if (!_stats.ContainsKey(key)) {
                    missingCount++;
                    if (missingCount <= MaxMissingTagWarnings) {
                        Logger.WarnFormat("Could not find tag {0}, using default {0}={1}", key, fallback.GetTag(key));
                    } else if (missingCount == MaxMissingTagWarnings + 1) {
                        Logger.WarnFormat("Suppressing further missing tag warnings ({0} so far)...", missingCount);
                    }
                    _stats[key] = fallback.GetTag(key);
                }
            }

            // Grim Dawn defines the item name ordering per language, eg German genders the prefix to match
            // the item name ("{%_3a0}..") where English simply concatenates. Missing means the game database
            // has not been parsed yet, in which case there are no localized item names to order either.
            if (!_stats.TryGetValue("tagItemNameOrder", out var itemNameOrder) || string.IsNullOrWhiteSpace(itemNameOrder)) {
                Logger.Warn("The parsed game tags contain no tagItemNameOrder, falling back to the English ordering");
                itemNameOrder = EnglishLanguage.ItemNameOrderFallback;
            }

            _itemCombinator = new ItemNameCombinator(itemNameOrder);
        }

        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            return _itemCombinator.TranslateName(prefix, quality, style, name, suffix);
        }

        public string TranslateName(string rawName) {
            return _itemCombinator.TranslateName(rawName);
        }

        public string GetTag(string tag) {
            if (_stats.ContainsKey(tag)) {
                return _stats[tag];
            }

            return string.Empty;
        }

        public string GetTag(string tag, object[] args) {
            return args.Select((t, i) => i)
                .Aggregate(GetTag(tag), (current, index) => current.Replace($"{{{index}}}", args[index]?.ToString()));
        }
    }
}
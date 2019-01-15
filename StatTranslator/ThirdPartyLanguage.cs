using log4net;
using System.Collections.Generic;
using System.Linq;

namespace StatTranslator
{
    public class ThirdPartyLanguage : ILocalizedLanguage {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThirdPartyLanguage));
        private readonly Dictionary<string, string> _stats;
        private readonly ItemNameCombinator _itemCombinator;
        public bool WarnIfMissing => true;

        public ThirdPartyLanguage(Dictionary<string, string> dataset, EnglishLanguage fallback) {
            _stats = dataset;

            // Make sure the loaded language has all the necessary keys
            foreach (var key in fallback.Serialize()) {
                if (!_stats.ContainsKey(key)) {
                    Logger.WarnFormat("Could not find tag {0}, using default {0}={1}", key, fallback.GetTag(key));
                    _stats[key] = fallback.GetTag(key);
                }
            }

            _itemCombinator = new ItemNameCombinator(dataset["tagItemNameOrder"]);
        }

        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            return _itemCombinator.TranslateName(prefix, quality, style, name, suffix);
        }

        public string GetTag(string tag)
        {
            if (_stats.ContainsKey(tag))
            {
                return _stats[tag];
            }

            return string.Empty;
        }

        public string GetTag(string tag, string arg)
        {
            return GetTag(tag, new[]
            {
                arg
            });
        }

        public string GetTag(string tag, string[] args)
        {
            return args.Select((t, i) => i)
                .Aggregate(GetTag(tag), (current, index) => current.Replace($"{{{index}}}", args[index]));
        }
    }
}

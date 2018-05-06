using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {
    public class ThirdPartyLanguage : ILocalizedLanguage {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ThirdPartyLanguage));
        private Dictionary<string, string> _stats = new Dictionary<string, string>();
        private ItemNameCombinator _itemCombinator;
        public bool WarnIfMissing => true;

        public ThirdPartyLanguage(Dictionary<string, string> dataset) {
            _stats = dataset;

            // Make sure the loaded language has all the necessary keys
            var english = new EnglishLanguage();
            foreach (var key in english.Serialize()) {
                if (!_stats.ContainsKey(key)) {
                    logger.WarnFormat("Could not find tag {0}, using default {0}={1}", key, english.GetTag(key));
                    _stats[key] = english.GetTag(key);
                }
            }

            _itemCombinator = new ItemNameCombinator(dataset["tagItemNameOrder"]);
        }


        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            return _itemCombinator.TranslateName(prefix, quality, style, name, suffix);
        }
        

        public string GetTag(string tag) {
            if (_stats.ContainsKey(tag))
                return _stats[tag];
            else
                return string.Empty;
        }

        public string GetTag(string tag, string arg1) {
            return GetTag(tag).Replace("{0}", arg1);
        }

        public string GetTag(string tag, string arg1, string arg2) {
            return GetTag(tag, arg1).Replace("{1}", arg2);
        }

        public string GetTag(string tag, string arg1, string arg2, string arg3) {
            return GetTag(tag, arg1, arg2).Replace("{2}", arg3);
        }
    }
}

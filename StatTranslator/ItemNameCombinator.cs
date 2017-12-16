using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StatTranslator {
    public class ItemNameCombinator {
        private readonly string tagItemNameOrder;
        public ItemNameCombinator(string tagItemNameOrder) {
            this.tagItemNameOrder = tagItemNameOrder;
        }

        public static string DetermineGender(string s) {
            if (s.Length > 4) {
                var tag = s.Substring(0, 4);
                if (tag.StartsWith("[") && tag.EndsWith("]")) {
                    return tag;
                }
            }

            return string.Empty;
        }

        public static string GetGendered(string s, string genderTag) {
            var tag = genderTag.Replace("[", @"\[").Replace("]", @"\]");
            var regex = new Regex(tag + @"(\w+)", RegexOptions.IgnoreCase);
            var match = regex.Match(s);
            if (match.Success)
                return match.Groups[1].Captures[0].Value;

            return s.Replace("$", "");
        }

        public static string FilterGenderTag(string s) {
            // Capitalization char
            if (s.Length > 0 && s[0] == '$')
                s = s.Remove(0);

            if (s.Length > 4 && s[0] == '[' && s[3] == ']')
                return s.Substring(4);
            else
                return s;
        }


        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            var entries = tagItemNameOrder.Split(@"\{%_".ToArray())
                .Where(m => !string.IsNullOrEmpty(m))
                .Select(m => m.Replace("}", ""));


            // #prefix/quality/style/name/suffix concatenation
            string[] fixes = {
                prefix, quality, style, name, suffix
            };

            string[] itemName = new string[fixes.Length];

            int pos = 0;
            foreach (var entry in entries) {
                int fixIndex;
                // NaN specifies this is a gender based entry
                if (entry.Length == 3 && char.IsDigit(entry[0])) {
                    int genderIndex;
                    if (int.TryParse(entry.Substring(0, 1), out genderIndex)) {
                        var genderTag = DetermineGender(fixes[genderIndex]);

                        if (int.TryParse(entry.Substring(2, 1), out fixIndex)) {
                            itemName[pos] = GetGendered(fixes[fixIndex], genderTag);
                        }
                    }

                    pos++;
                }
                // 'sN'
                else if (entry.Length == 2) {
                    if (int.TryParse(entry.Replace("s", ""), out fixIndex))
                        itemName[pos] = FilterGenderTag(fixes[fixIndex]);
                    
                    pos++;
                }
            }
            
            return string.Join(" ", itemName.Where(m => !string.IsNullOrEmpty(m)).ToList());
        }
    }
}

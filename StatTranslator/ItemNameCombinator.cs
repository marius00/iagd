using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace StatTranslator
{
    public class ItemNameCombinator {
        private readonly string _tagItemNameOrder;

        public ItemNameCombinator(string tagItemNameOrder) {
            _tagItemNameOrder = tagItemNameOrder;
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
            var tag = Regex.Escape(genderTag);
            var match = Regex.Match(s, tag + @"(?<gendered>\w+)", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Groups["gendered"].Value;
            }

            return s.Replace("$", "");
        }

        /// <summary>
        /// Determines gender in the <paramref name="parts"/> array.
        /// <param name="parts">An array, which represents the original name, splitted by the space.</param>
        /// </summary>
        public static string DetermineGender(string[] parts) {
            for (int i = 0; i < parts.Length; ++i)
            {
                var part = parts[i].Trim();

                // Check if the part starts with [XX]
                if (part.Length >= 4
                    && part[0] == '['
                    && part[3] == ']')
                {
                    var isMultiGenderPart = part.IndexOf('[', 1) > 0;

                    if (isMultiGenderPart)
                    {
                        // Multigender part - skip
                        continue;
                    }

                    return part.Substring(0, 4);
                }
            }

            return null;
        }

        public static string FilterGenderTag(string s) {
            // Capitalization char
            if (s.Length > 0 && s[0] == '$')
            {
                s = s.Remove(0);
            }

            if (s.Length > 4 && s[0] == '[' && s[3] == ']')
            {
                return s.Substring(4);
            }

            return s;
        }

        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            var entries = _tagItemNameOrder.Split(@"\{%_".ToArray())
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
                    if (int.TryParse(entry.Replace("s", "").Replace("\r", ""), out fixIndex)) {
                        itemName[pos] = FilterGenderTag(fixes[fixIndex]);
                    }

                    pos++;
                }
            }
            
            return string.Join(" ", itemName.Where(m => !string.IsNullOrEmpty(m)).ToList());
        }

        public string TranslateName(string rawName) {
            if (string.IsNullOrEmpty(rawName))
            {
                return rawName;
            }

            var parts = rawName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var genderTag = DetermineGender(parts);

            // Gender not determined - return original name
            if (string.IsNullOrEmpty(genderTag))
            {
                return rawName;
            }

            for (int i = 0; i < parts.Length; ++i)
            {
                parts[i] = GetGendered(parts[i], genderTag);
            }

            return string.Join(" ", parts);
        }
    }
}

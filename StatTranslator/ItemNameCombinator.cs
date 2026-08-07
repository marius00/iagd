using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StatTranslator
{
    public class ItemNameCombinator {
        /// <summary>
        /// Grim Dawn keeps every gendered form of a name in a single tag, eg
        /// "[ms]Mächtiger[fs]Mächtige[ns]Mächtiges[mp]Mächtige[fp]Mächtige[np]Mächtige".
        /// The two letter code is gender + count, eg "fp" = feminine plural. A variant runs until the next
        /// marker and may contain spaces ("[ms]Des Inquisitors[fs]Des Inquisitors..").
        /// </summary>
        private static readonly Regex GenderMarker = new Regex(@"\[(?<code>[a-zA-Z]{2})\]", RegexOptions.Compiled);

        private static readonly Regex RepeatedWhitespace = new Regex(@"\s{2,}", RegexOptions.Compiled);

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

        /// <summary>
        /// Pick the <paramref name="genderTag"/> ("[fs]") variant out of a gendered tag.
        /// </summary>
        public static string GetGendered(string s, string genderTag) {
            var gender = genderTag?.Trim('[', ']');

            return Resolve(s, string.IsNullOrEmpty(gender) ? null : gender);
        }

        /// <summary>
        /// Reduce a gendered tag to a plain name, keeping the first variant.
        /// </summary>
        public static string FilterGenderTag(string s) {
            return Resolve(s, null);
        }

        /// <summary>
        /// One gendered tag: the variants it offers, in the order the game lists them.
        /// </summary>
        private readonly struct GenderedTag {
            public GenderedTag(int start, int end, List<string> codes, List<string> variants) {
                Start = start;
                End = end;
                Codes = codes;
                Variants = variants;
            }

            /// <summary>Index of the tag's first marker.</summary>
            public int Start { get; }

            /// <summary>Index just past the tag's last variant.</summary>
            public int End { get; }

            public List<string> Codes { get; }
            public List<string> Variants { get; }

            /// <summary>
            /// The form for <paramref name="gender"/>, or the first form when it offers none. Not every tag
            /// carries every gender, and an untranslated name carries no gender to ask for at all.
            /// </summary>
            public string Get(string? gender) {
                if (gender != null) {
                    for (var i = 0; i < Codes.Count; i++) {
                        if (string.Equals(Codes[i], gender, StringComparison.OrdinalIgnoreCase)) {
                            return Variants[i];
                        }
                    }
                }

                return Variants.Count > 0 ? Variants[0] : string.Empty;
            }
        }

        /// <summary>
        /// Split out the gendered tags in a (possibly concatenated) name. Markers run back to back within a
        /// tag, so the boundary between two adjacent tags is a gender the current tag already listed -- no
        /// tag names the same gender twice.
        /// </summary>
        private static List<GenderedTag> ParseGenderedTags(string s) {
            var result = new List<GenderedTag>();
            var markers = GenderMarker.Matches(s);

            var i = 0;
            while (i < markers.Count) {
                var codes = new List<string>();
                var variants = new List<string>();

                while (i < markers.Count) {
                    var code = markers[i].Groups["code"].Value;
                    if (codes.Any(c => string.Equals(c, code, StringComparison.OrdinalIgnoreCase))) {
                        break;
                    }

                    var variantStart = markers[i].Index + markers[i].Length;
                    var variantEnd = i + 1 < markers.Count ? markers[i + 1].Index : s.Length;

                    codes.Add(code);
                    variants.Add(s.Substring(variantStart, variantEnd - variantStart));
                    i++;
                }

                var start = markers[i - codes.Count].Index;
                var end = i < markers.Count ? markers[i].Index : s.Length;
                result.Add(new GenderedTag(start, end, codes, variants));
            }

            return result;
        }

        /// <summary>
        /// Replace every gendered tag in <paramref name="s"/> with its <paramref name="gender"/> variant.
        /// Nothing downstream understands the markers, and a stray "[" is read as the start of a component
        /// name -- which is how a German item ended up displayed as "Mächtiger (fsMächtige)".
        /// </summary>
        private static string Resolve(string s, string? gender) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

            if (!s.Contains("[")) {
                return Clean(s);
            }

            var tags = ParseGenderedTags(s);
            if (tags.Count == 0) {
                return Clean(s);
            }

            var sb = new StringBuilder(s.Length);
            var pos = 0;
            foreach (var tag in tags) {
                if (tag.Start == pos && pos > 0) {
                    // Two tags back to back. Whatever separated them lived at the end of the previous
                    // tag's last variant, and picking any other variant just dropped it.
                    sb.Append(' ');
                }

                sb.Append(s, pos, tag.Start - pos);
                sb.Append(tag.Get(gender));
                pos = tag.End;
            }

            sb.Append(s, pos, s.Length - pos);

            return Clean(sb.ToString());
        }

        private static string Clean(string s) {
            return RepeatedWhitespace.Replace(s.Replace("$", ""), " ").Trim();
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

        /// <summary>
        /// Join tag values into one name, gendered to agree with each other. The item name is the only part
        /// with a single gendered form, so it is what the styles and qualities around it have to agree with.
        /// Prefer this over <see cref="TranslateName(string)"/> whenever the individual tag values are still
        /// available: once they are concatenated, a name that carries no gender of its own (an untranslated
        /// tag) can no longer be told apart from the trailing variant of the tag before it.
        /// </summary>
        public static string Combine(params string?[] tagValues) {
            var values = tagValues.Where(v => !string.IsNullOrEmpty(v)).Select(v => v!).ToList();

            var gender = values
                .Select(ParseGenderedTags)
                .SelectMany(tags => tags)
                .FirstOrDefault(tag => tag.Codes.Count == 1)
                .Codes?.FirstOrDefault();

            return string.Join(" ", values.Select(v => Resolve(v, gender)).Where(v => !string.IsNullOrEmpty(v)));
        }

        /// <summary>
        /// Resolve an already concatenated name. Best effort -- see <see cref="Combine"/>.
        /// </summary>
        public string TranslateName(string rawName) {
            if (string.IsNullOrEmpty(rawName) || !rawName.Contains("[")) {
                return rawName;
            }

            var gender = ParseGenderedTags(rawName)
                .FirstOrDefault(tag => tag.Codes.Count == 1)
                .Codes?.FirstOrDefault();

            return Resolve(rawName, gender);
        }
    }
}

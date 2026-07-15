namespace StatTranslator
{
    public enum TranslatedStatType {
        HEADER, BODY, FOOTER, PET, SKILL
    };

    public class TranslatedStat {
        public string? Text;
        public float? Param0;
        public float? Param1;
        public float? Param2;

        public string? Param3;
        public float? Param4;
        public string? Param5;
        public string? Param6;

        public TranslatedStatType Type;

        /// <summary>
        /// This is used to embed extra data, like embedding "Tier 4 Occultist" on a +2 to SomeSkill
        /// </summary>
        public TranslatedStat? Extra;

        public override string ToString() {
            var text = Text;
            if (text == null) {
                return null;
            }

            text = ReplaceNumeric(text, "{0}", Param0);
            text = ReplaceNumeric(text, "{1}", Param1);
            text = ReplaceNumeric(text, "{2}", Param2);
            text = text.Replace("{3}", Param3);
            text = ReplaceNumeric(text, "{4}", Param4);
            text = text.Replace("{5}", Param5);
            text = text.Replace("{6}", Param6);
            return text;
        }

        /// <summary>
        /// Replaces a numeric placeholder. When the placeholder is immediately followed by a '%'
        /// (i.e. the value is a percentage), the value is rounded to the nearest whole integer so
        /// seed-calculated stats don't render as "17.831049% Physical Damage converted to Cold".
        /// </summary>
        private static string ReplaceNumeric(string text, string placeholder, float? value) {
            if (value == null) {
                return text;
            }

            var idx = text.IndexOf(placeholder, System.StringComparison.Ordinal);
            var isPercentage = idx >= 0
                && idx + placeholder.Length < text.Length
                && text[idx + placeholder.Length] == '%';

            var formatted = isPercentage
                ? System.Math.Round(value.Value, System.MidpointRounding.AwayFromZero)
                    .ToString("0", System.Globalization.CultureInfo.InvariantCulture)
                : value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return text.Replace(placeholder, formatted);
        }
    }
}

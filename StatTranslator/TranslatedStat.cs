using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {

    public enum TranslatedStatType {
        HEADER, BODY, FOOTER, PET, SKILL
    };

    public class TranslatedStat {
        public string Text;
        public float? Param0;
        public float? Param1;
        public float? Param2;

        public string Param3;
        public float? Param4;
        public string Param5;
        public string Param6;

        public TranslatedStatType Type;

        /// <summary>
        /// This is used to embed extra data, like embedding "Tier 4 Occultist" on a +2 to SomeSkill
        /// </summary>
        public TranslatedStat Extra;
        
        

        public override string ToString() {
            return Text?.Replace("{0}", Param0?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{1}", Param1?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{2}", Param2?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{3}", Param3)
                    .Replace("{4}", Param4?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{5}", Param5)
                    .Replace("{6}", Param6)
                    ;
        }
    }
}

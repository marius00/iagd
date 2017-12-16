using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EvilsoftCommons {
    class StringUtilities {
        public static string RemoveDiacritics(string text) {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ToCurrency(double value) {
            return string.Format("R$ {0:0.00}", value);
        }

        public static string ToCPF(long cpf) {
            if (cpf == 0)
                return string.Empty;
            else
                return string.Format(@"{0:###\.###\.###-##}", cpf);
        }

        public static string ToCEP(long cep) {
            if (cep == 0)
                return "";
            else
                return string.Format(@"{0:##\.###-###}", cep);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {
    public interface ILocalizedLanguage {

        string GetTag(string tag);
        string GetTag(string tag, string arg1);
        string GetTag(string tag, string arg1, string arg2);

        string TranslateName(string prefix, string quality, string style, string name, string suffix);

        bool WarnIfMissing { get; }
    }
}

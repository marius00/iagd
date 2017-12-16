using IAGrim.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities.HelperClasses {


    public class GDTransferFile : ComboBoxItemToggle {
        public string Filename { get; set; }
        public bool IsHardcore { get; set; }
        public string Mod { get; set; }
        public virtual bool Enabled { get; set; }

        public bool IsExpansion1 { get; set; }

        public virtual DateTime LastAccess { get; set; }

        
        public override string ToString() {
            string text = string.IsNullOrEmpty(Mod) ? GlobalSettings.Language.GetTag("iatag_ui_vanilla") : Mod;
            if (this.IsHardcore)
                return $"{text}{GlobalSettings.Language.GetTag("iatag_ui_hc")}";
            else
                return text;
        }
    }

}

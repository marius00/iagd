using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Theme {
    class ComboBoxItemQuality {
        public int PrefixRarity {
            get;
            set;
        }

        public string Rarity {
            get;
            set;
        }

        public string Text {
            get;
            set;
        }

        public override string ToString() {
            return Text;
        }
    }
}

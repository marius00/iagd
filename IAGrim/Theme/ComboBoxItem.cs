using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Theme {
    class ComboBoxItem {
        public string[] Filter {
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

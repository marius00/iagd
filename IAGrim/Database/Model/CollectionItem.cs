using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Model {
    public class CollectionItem {
        public string BaseRecord { get; set; }
        public string Name { get; set; }
        public long NumOwnedSc { get; set; }
        public long NumOwnedHc { get; set; }
        public string Quality { get; set; }

        private string _icon;
        public string Icon {
            get => (_icon?.Substring(1 + _icon.LastIndexOf("/")) + ".png") ?? string.Empty;
            set => _icon = value;
        }
    }
}

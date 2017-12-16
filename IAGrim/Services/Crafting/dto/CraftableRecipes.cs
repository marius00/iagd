using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.Crafting.dto {
    class CraftableRecipes {
        public List<ComponentListEntry> Relics { get; set; }
        public List<ComponentListEntry> Misc { get; set; }
        public List<ComponentListEntry> Components { get; set; }
    }
}

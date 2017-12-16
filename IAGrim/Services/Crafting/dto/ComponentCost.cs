using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.Crafting.dto {
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable CollectionNeverQueried.Global
    class ComponentCost {
        public string Name { get; set; }

        public string Bitmap {
            get; set;
        }

        public string Record { get; set; }

        public int NumRequired { get; set; }

        public int NumOwned { get; set; }

        public int NumCraftable { get; set; }

        public bool IsComplete { get; set; }

        public IList<ComponentCost> Cost { get; set; }
    }
    // ReSharper enable UnusedAutoPropertyAccessor.Global
    // ReSharper enable CollectionNeverQueried.Global
}

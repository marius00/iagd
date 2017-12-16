using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    public interface IItemTag {
        string Tag { get; }
        string Name { get; }
    }

    class ItemTag : IItemTag {
        public virtual string Tag { get; set; }
        public virtual string Name { get; set; }
    }
}

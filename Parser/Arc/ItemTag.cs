using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    public interface IItemTag {
        string? Tag { get; }
        string? Name { get; }
    }

    class ItemTag : IItemTag {
        public required virtual string Tag { get; set; }
        public required virtual string Name { get; set; }
    }
}

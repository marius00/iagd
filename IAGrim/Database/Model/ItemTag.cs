using IAGrim.Parser.Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    public class ItemTag : IItemTag, IComparable<ItemTag> {
        public virtual string Tag { get; set; }
        public virtual string Name { get; set; }

        public virtual int CompareTo(ItemTag other) {
            return Tag.CompareTo(other.Tag);
        }

        public override bool Equals(object obj) {
            ItemTag that = obj as ItemTag;
            if (that != null)
                return Tag.Equals(that.Tag);

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Tag.GetHashCode();
        }
    }
}

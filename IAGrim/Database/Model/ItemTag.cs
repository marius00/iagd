using IAGrim.Parser.Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    public class ItemTag : IItemTag, IComparable<ItemTag> {
        public virtual string? Tag { get; set; }
        public virtual string? Name { get; set; }

        public virtual int CompareTo(ItemTag? other) {
            if (other == null) {
                return 1;
            }

            return string.Compare(Tag, other.Tag, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) {
            ItemTag? that = obj as ItemTag;
            if (that != null)
                return Tag?.Equals(that.Tag) ?? that.Tag == null;

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Tag?.GetHashCode() ?? 0;
        }
    }
}

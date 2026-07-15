using System;
using System.Collections.Generic;
using DataAccess;

namespace IAGrim.Parsers.Arz.dto {
    public class ItemGrantedSkill : IComparable<ItemGrantedSkill> {
        public string? Description;

        public long? Level;

        public string? Name;
        public string? Record;

        public long? StatsId;

        public string? Trigger;

        public int CompareTo(ItemGrantedSkill? obj) {
            if (obj == null) return 1;
            return string.Compare(Record, obj.Record, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) {
            ItemGrantedSkill? that = obj as ItemGrantedSkill;
            if (that != null)
                return string.Equals(Record, that.Record);

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Record?.GetHashCode() ?? 0;
        }
    }
}
using DataAccess;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IAGrim.Database.Model;

namespace IAGrim.Database {
    /// <summary>
    /// Lookup table for records.
    /// Simplifies lookups, not having to do "OR BaseRecord OR PrefixRecord OR....", and may contain optional records such as petbonuses
    /// </summary>
    public class BuddyItemRecord {
        public virtual string ItemId { get; set; }
        public virtual string Record { get; set; }

        public override bool Equals(object obj) {
            if (obj is BuddyItemRecord that) {
                return this.ItemId == that.ItemId && this.Record == that.Record;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (ItemId + Record).GetHashCode();
        }
    }
}


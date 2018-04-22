using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database {
    /// <summary>
    /// Represents a deleted player item which has an online-presence.
    /// Used to delete items from the online backup.
    /// </summary>
    public class DeletedPlayerItem {
        public virtual string Id { get; set; }
        public virtual string Partition { get; set; }

        public override bool Equals(object obj) {
            if (obj is DeletedPlayerItem that) {
                return this.Id == that.Id && this.Partition == that.Partition;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (Id + Partition).GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database {
    public class PlayerItemRecord {
        public virtual long PlayerItemId { get; set; }
        public virtual string Record { get; set; }

        public override bool Equals(object obj) {
            PlayerItemRecord that = obj as PlayerItemRecord;
            if (that != null) {
                return this.PlayerItemId == that.PlayerItemId && this.Record == that.Record;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (PlayerItemId + Record).GetHashCode();
        }
    }
}

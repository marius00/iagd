using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Parser.Arz;

namespace IAGrim.Database {
    public class AzurePartition : IComparable<AzurePartition> {
        public virtual string Id { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual int CompareTo(AzurePartition other) {
            return String.Compare(Id, other.Id, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) {
            if (obj is AzurePartition other)
                return Id.Equals(other.Id);

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
        public override string ToString() {
            return $"AzurePartition[{Id}, {IsActive}]";
        }
    }
}

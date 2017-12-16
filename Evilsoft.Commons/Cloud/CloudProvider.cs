using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.Cloud {
    public class CloudProvider : IComparable<CloudProvider> {
        public CloudProviderEnum Provider;
        public string Location;

        public int CompareTo(CloudProvider obj) {
            return Location.CompareTo(obj.Location);
        }

        public override bool Equals(object obj) {
            CloudProvider o = obj as CloudProvider;
            if (o != null)
                return Location.Equals(o.Location);
            else
                return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Location.GetHashCode();
        }
    }
}

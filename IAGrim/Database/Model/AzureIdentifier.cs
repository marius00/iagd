using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Model {
    class AzureIdentifier {
        public virtual string Partition { get; set; }
        public virtual string Uuid { get; set; }
    }
}

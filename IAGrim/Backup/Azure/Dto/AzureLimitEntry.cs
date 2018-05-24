using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class AzureLimitEntry {
        public long Download { get; set; }
        public long Upload { get; set; }
        public long Delete { get; set; }
    }
}

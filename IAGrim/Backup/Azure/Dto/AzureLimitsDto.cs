using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class AzureLimitsDto {
        public AzureLimitEntry Regular { get; set; }
        public AzureLimitEntry MultiUsage { get; set; }
    }
}

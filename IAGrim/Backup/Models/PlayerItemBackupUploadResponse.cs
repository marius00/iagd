using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Models {

#pragma warning disable 0649
    class PlayerItemBackupUploadResponse {
        public bool Success;
        public int ErrorCode;
        public long OID;
        public long Modified;
    }
#pragma warning restore 0649
}

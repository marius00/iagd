using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    public static class SeqGuid {
        public static Guid Create() {
            const int RPC_S_OK = 0;
            const int RPC_S_UUID_LOCAL_ONLY = 1824;
            Guid g;
            int hr = UuidCreateSequential(out g);
            if (hr != RPC_S_OK && hr != RPC_S_UUID_LOCAL_ONLY) {
                throw new ApplicationException("UuidCreateSequential failed: " + hr);
            }

            return g;
        }

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities.Detection {
    static class BlockedLogsDetection {
        private static bool IsLocalDomain(string domain) {
            try {
                var ip = Dns.GetHostEntry(domain)?.AddressList[0].ToString();
                return "127.0.0.1".Equals(ip) || "::1".Equals(ip);
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// User has blocked connections to logging and statistics.
        /// Useful for not not even bother to try.
        /// </summary>
        public static bool DreamcrashBlocked() => IsLocalDomain("ribbs.dreamcrash.org");


    }
}

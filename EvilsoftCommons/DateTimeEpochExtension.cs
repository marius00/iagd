using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilsoftCommons {
    public static class DateTimeEpochExtension {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToTimestamp(this DateTime value) {

            TimeSpan elapsedTime = value - Epoch;
            long result = (long)elapsedTime.TotalMilliseconds;
            return result > 0 ? result : 0;
        }
        /*
                public static DateTime ToDateTime(this long value) {
                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(value).ToLocalTime();
                    return dtDateTime;
                }*/
        public static DateTime FromTimestamp(long value) {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(value).ToLocalTime();
            return dtDateTime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Models {
    class PlayerItemResponseCodes {
        public const int ERRORCODE_SUCCESS = 1;
        public const int ERRORCODE_AUTH_FAILURE = 2;
        public const int ERRORCODE_INVALID_ITEM = 3;

        /// <summary>
        /// Token is valid, but has not yet been confirmed via the key.
        /// </summary>
        public const int ERRORCODE_NOT_AUTHORIZED = 4;

        /// <summary>
        /// "Something went wrong" sending emails.
        /// Maybe it'll resolve itself.
        /// </summary>
        public const int ERRORCODE_TRY_AGAIN = 5;

        /// <summary>
        /// Excessive requests from an IP
        /// </summary>                  
        public const int ERRORCODE_THROTTLED = 6;

        /// <summary>
        /// E-mail did not pass regex validation, appears to be invalid.
        /// </summary>
        public const int ERRORCODE_INVALID_EMAIL = 7;
    }
}

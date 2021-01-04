using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    /// <summary>
    /// List of buddies who the user subscribes to
    /// </summary>
    public class BuddySubscription {
        public virtual long Id { get; set; }
        public virtual string Nickname { get; set; }
        public virtual long LastSyncTimestamp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    /// <summary>
    /// User id and description for a stash
    /// </summary>
    public class BuddyStash {
        public virtual long UserId { get; set; }
        public virtual string Description { get; set; }
    }
}

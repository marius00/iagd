using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAGrim.BuddyShare.dto;

namespace IAGrim.BuddyShare {
    public class SerializedPlayerItems {
        public string Items { get; set; }
        public string Verification { get; set; }
        public string Description { get; set; }
        public string UUID { get; set; }
        public long UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.BuddyShare.dto {
    public class BuddyUploadResponse {
        public string Content { get; set; }
        public int Status { get; set; }

        public bool Success => Status == 200 && !(Content.Contains("\"failure\"") && Content.Length < 50);
    }
}

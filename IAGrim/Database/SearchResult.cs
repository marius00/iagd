using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database {
    public class SearchResult {
        public List<PlayerHeldItem> Items { get; set; }
        public int NumResults { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.ItemReplica {
    class ReplicaCache {
        private readonly Dictionary<string, byte> _cache = new Dictionary<string, byte>();

        public void Add(string hash) {
            _cache.Add(hash, 0);

            // TODO: In the future, delete the oldest or something
            if (_cache.Keys.Count > 10000)
                _cache.Clear();
        }

        public bool Exists(string hash) {
            return _cache.ContainsKey(hash);
        }
    }
}

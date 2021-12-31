using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.ItemReplica {
    class ReplicaCache {
        private readonly Dictionary<int, byte> _cache = new Dictionary<int, byte>();

        public void Add(int hash) {
            _cache.Add(hash, 0);

            // TODO: In the future, delete the oldest or something
            if (_cache.Keys.Count > 10000)
                _cache.Clear();
        }

        public bool Exists(int hash) {
            return _cache.ContainsKey(hash);
        }
    }
}

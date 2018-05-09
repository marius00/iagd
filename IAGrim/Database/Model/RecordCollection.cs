using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Model {
    interface RecordCollection {
        string BaseRecord { get; }
        string PrefixRecord { get; }
        string SuffixRecord { get; }
        string MateriaRecord { get; }

    }
}

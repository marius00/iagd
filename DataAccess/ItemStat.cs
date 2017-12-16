using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess {
/*
    public interface IItemStat {
        string Record { get; }
        string Stat { get; }

        // Either a float or an int
        float Value { get; }
        string TextValue { get; }
    }
*/

    public class ItemStat : IItemStat {
        public virtual long Id { get; set; }
        public string Stat { get; set; }
        public float Value { get; set; }
        public string TextValue { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess {
    public interface IItemStat {
        //long Id { get; set; }

        string Stat { get; set; }
        float Value { get; set; }
        string TextValue { get; set; }

    }
    public interface IItemStatI : IItemStat {
        long Id { get; set; }

    }
    public interface IItemStatR : IItemStat {
        string Record { get; set; }

    }
}

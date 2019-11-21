using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess {
    public interface IItemStat {

        string Record { get; set; }
        string Stat { get; set; }
        float Value { get; set; }
        string TextValue { get; set; }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess {

/*
    public interface IItem {
        string Record { get; }
        ICollection<IItemStat> Stats { get; }
    }*/

    public class Item : IItem {
        public string Record {
            get; set;
        }

        public ICollection<IItemStat> Stats {
            get; set;
        }
    }
}

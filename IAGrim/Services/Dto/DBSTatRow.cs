using DataAccess;
using IAGrim.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.Dto {

    public class DBSTatRow : IItemStatI {
        public string Record { get; set; }
        public string Stat { get; set; }
        public double Value { get; set; }
        public string TextValue { get; set; }

        float IItemStat.Value {
            get {
                return (float)Value;
            }

            set {
                this.Value = value;
            }
        }

        public long Id { get; set; }

        public DatabaseItemStat ToStat() {
            return new DatabaseItemStat {
                Stat = Stat,
                TextValue = TextValue,
                Value = (float)Value
            };
        }
    }
}

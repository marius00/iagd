using DataAccess;
using IAGrim.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services.Dto {

    public class DBStatRow : IItemStat {
        public string Record { get; set; }
        public string Stat { get; set; }
        public double Value { get; set; }
        public string TextValue { get; set; }

        float IItemStat.Value {
            get => (float)Value;

            set => this.Value = value;
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

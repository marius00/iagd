using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Interfaces {
    public interface IItemTagDao : IBaseDao<ItemTag> {
        void Save(ICollection<ItemTag> items, ProgressTracker tracker);
        IList<ItemTag> GetClassItemTags();
        IList<ItemTag> GetValidClassItemTags();
    }
}

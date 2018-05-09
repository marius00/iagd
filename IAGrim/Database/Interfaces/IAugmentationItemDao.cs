using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;

namespace IAGrim.Database.Interfaces {
    interface IAugmentationItemDao : IBaseDao<AugmentationItem> {
        void UpdateState();
        IList<AugmentationItem> Search(Search query);
    }
}

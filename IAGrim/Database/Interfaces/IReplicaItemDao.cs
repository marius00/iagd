using IAGrim.Database.Model;
using System.Collections.Generic;

namespace IAGrim.Database.Interfaces {
    public interface IReplicaItemDao : IBaseDao<ReplicaItem> {
        void Save(ReplicaItem obj, List<ReplicaItemRow> rows);
        void DeleteAll();
    }
}
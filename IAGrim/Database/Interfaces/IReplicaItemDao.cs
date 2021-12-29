using IAGrim.Database.Model;

namespace IAGrim.Database.Interfaces {
    public interface IReplicaItemDao : IBaseDao<ReplicaItem> {
        void UpdatePlayerItemId(int uqHash, long playerItemId);
    }
}
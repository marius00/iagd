using System.Collections.Generic;
using IAGrim.Database.Model;
using IAGrim.Services.ItemReplica;
using IAGrim.UI.Controller.dto;

namespace IAGrim.UI.Misc.CEF {
    public interface IBrowserCallbacks {
        void AddItems(List<List<JsonItem>> items, bool hasMore, int numItemsFound = -1);
        void SetItems(List<List<JsonItem>> items, int numItemsFound, bool hasMore, bool numItemsApproximate = false);
        void SetCollectionItems(IList<CollectionItem> items, bool isHardcore);
        void SetCollectionAggregateData(IList<CollectionItemAggregateRow> rows);
        void ShowLoadingAnimation(bool visible);
        void ShowModFilterWarning(int numOtherItems);
        bool IsReady();

        void SignalCloudIconChange(IList<long> playerItemIds);
        void SignalReplicaStatChange(long playerItemId, IList<ItemStatInfo> stats);
    }
}

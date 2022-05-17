using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Model;
using IAGrim.UI.Controller.dto;

namespace IAGrim.UI.Misc.CEF {
    public interface IBrowserCallbacks {
        void AddItems(List<List<JsonItem>> items);
        void SetItems(List<List<JsonItem>> items, int numItemsFound);
        void SetCollectionItems(IList<CollectionItem> items);
        void SetCollectionAggregateData(IList<CollectionItemAggregateRow> rows);
        void ShowLoadingAnimation(bool visible);
    }
}

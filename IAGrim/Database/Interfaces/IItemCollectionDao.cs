using System.Collections.Generic;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;


namespace IAGrim.Database.Interfaces {
    public interface IItemCollectionDao {
        IList<CollectionItem> GetItemCollection(ItemSearchRequest query);
        IList<CollectionItemAggregateRow> GetItemAggregateStats();
    }
}

using System.Collections.Generic;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;

namespace IAGrim.Database.Interfaces {
    public interface IDatabaseItemDao : IBaseDao<DatabaseItem> {
        void Save(ICollection<ItemTag> items);
        void SaveOrUpdate(ICollection<ItemTag> items);
        Dictionary<string, string> GetTagDictionary();
        void SaveOrUpdate(ICollection<DatabaseItem> items);
        void Save(ICollection<DatabaseItem> items);
        DatabaseItem FindByRecord(string record);
        IList<string> ListAllRecords();


        long GetRowCount();
        IList<RecipeItem> SearchForRecipeItems(Search query);

        IList<ItemTag> GetClassItemTags();
        IList<ItemTag> GetValidClassItemTags();

        DatabaseItemDto FindDtoByRecord(string record);
        List<DatabaseItemDto> GetCraftableItems();
        List<DatabaseItemDto> GetByClass(string itemClass);
    }
}
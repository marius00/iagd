using System.Collections.Generic;

namespace IAGrim.Database.Interfaces {
    public interface IRecipeItemDao : IBaseDao<RecipeItem> {
        void DeleteAll(bool isHardcore);
        IList<DatabaseItemStat> GetRecipeStats(ICollection<string> formulas);
        IList<DatabaseItemStat> GetRecipeItemStats(ICollection<string> items);
    }
}
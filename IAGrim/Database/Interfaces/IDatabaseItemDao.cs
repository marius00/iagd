using System.Collections.Generic;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Interfaces {
    public interface IDatabaseItemDao : IBaseDao<DatabaseItem> {
        Dictionary<string, string> GetTagDictionary();
        void Save(List<DatabaseItem> items, ProgressTracker progressTracker);
        void CreateItemIndexes(ProgressTracker progressTracker);
        IList<string> ListAllRecords();

        long GetRowCount();
        IList<ItemSetAssociation> GetItemSetAssociations();
        IList<string> GetSpecialStackableRecords();
        IList<string> GetStackableComponentsPotionsMisc();

        void Clean();
    }
}
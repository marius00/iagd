using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Stash;

namespace IAGrim.Parsers.TransferStash {
    public class TransferStashServiceCache {
        private readonly IDatabaseItemDao _databaseItemDao;
        /// <summary>
        /// Typical "single seed" items such as Dynamite
        /// </summary>
        public IList<string> SpecialRecords { get; private set; }
        public ISet<string> AllRecords { get; private set; }
        public ISet<string> StackableRecords { get; private set; }

        public TransferStashServiceCache(IDatabaseItemDao databaseItemDao) {
            _databaseItemDao = databaseItemDao;
            Refresh();
        }

        public void Refresh() {
            var special = new List<string>();
            special.AddRange(StashTab.HardcodedRecords);
            special.AddRange(_databaseItemDao.GetSpecialStackableRecords());
            SpecialRecords = special;
            AllRecords = new HashSet<string>(_databaseItemDao.ListAllRecords());
            StackableRecords = new HashSet<string>(_databaseItemDao.GetStackableComponentsPotionsMisc());
        }
    }
}

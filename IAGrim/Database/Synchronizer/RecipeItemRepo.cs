﻿using IAGrim.Database.DAO;
using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class RecipeItemRepo : BasicSynchronizer<RecipeItem>, IRecipeItemDao {
        private readonly IRecipeItemDao repo;
        public RecipeItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            this.repo = new RecipeItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }

        public void DeleteAll(bool isHardcore) {
            ThreadExecuter.Execute(
                () => repo.DeleteAll(isHardcore)
            );
        }
        public IList<DatabaseItemStat> GetRecipeItemStats(ICollection<string> items) {
            return ThreadExecuter.Execute(
                () => repo.GetRecipeItemStats(items)
            );
        }

        public IList<DatabaseItemStat> GetRecipeStats(ICollection<string> formulas) {
            return ThreadExecuter.Execute(
                () => repo.GetRecipeStats(formulas)
            );
        }
    }
}

using IAGrim.Database.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.DAO {
    public class RecipeItemDaoImpl : BaseDao<RecipeItem>, IRecipeItemDao {
        public RecipeItemDaoImpl(ISessionCreator sessionCreator) : base(sessionCreator) {
        }

        public void DeleteAll(bool isHardcore) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (isHardcore) {
                        session.CreateQuery("DELETE FROM RecipeItem WHERE IsHardcore").ExecuteUpdate();
                    }
                    else {
                        session.CreateQuery("DELETE FROM RecipeItem WHERE NOT IsHardcore").ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }



        

        public IList<DatabaseItemStat> GetRecipeItemStats(ICollection<string> items) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var subquery = Subqueries.PropertyIn("P.Id", DetachedCriteria.For<DatabaseItem>()
                        .Add(Restrictions.In("Record", items.Take(Math.Min(items.Count, 950)).ToArray()))
                        .SetProjection(Projections.Property("Id"))
                        );

                    string[] desiredStats = new string[] { "levelRequirement" };

                    return session.CreateCriteria<DatabaseItemStat>()
                            .Add(subquery)
                            .Add(Restrictions.In("Stat", desiredStats))
                            .CreateAlias("Parent", "P")
                            .AddOrder(Order.Desc("Stat"))
                            .List<DatabaseItemStat>();
                }
            }
        }

        public IList<DatabaseItemStat> GetRecipeStats(ICollection<string> formulas) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var subquery = Subqueries.PropertyIn("P.Id", DetachedCriteria.For<DatabaseItem>()
                        .Add(Restrictions.In("Record", formulas.ToArray()))
                        .SetProjection(Projections.Property("Id"))
                        );

                    var desiredStats = new[] { "artifactName", "forcedRandomArtifactName" };

                    return session.CreateCriteria<DatabaseItemStat>()
                            .Add(subquery)
                            .Add(Restrictions.In("Stat", desiredStats))
                            .CreateAlias("Parent", "P")
                            .AddOrder(Order.Desc("Stat"))
                            .List<DatabaseItemStat>();
                }
            }
        }
    }
}

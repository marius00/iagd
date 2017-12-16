using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IAGrim.Database;
using NHibernate;
using NHibernate.Criterion;
using IAGrim.Database.Interfaces;

namespace Tests_IAGrim.Dao {

    [TestClass]
    public class DatabaseItemDaoTests {
        private static IAGrim.Tests.SessionFactory _sessionCreator = new IAGrim.Tests.SessionFactory();
        private IDatabaseItemDao _dao = new IAGrim.Database.DatabaseItemDaoImpl(_sessionCreator);

        [TestMethod]
        public void CanSaveSingleItemWithStats() {
            var item = new DatabaseItem {
                Name = "Magic item",
                Record = "test/record/with/stats"
            };

            List<DatabaseItemStat> stats = new List<DatabaseItemStat>();
            item.Stats = stats;

            stats.Add(new DatabaseItemStat {
                //     Parent = item,
                Stat = "TestStat",
                TextValue = "Value"
            });

            _dao.Save(item);

            item.Id.Should().Be.GreaterThan(0);
            item.Stats.First().Id.Should().Be.GreaterThan(0);



            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    DatabaseItem loaded = session
                        .CreateCriteria<DatabaseItem>()
                        .Add(Restrictions.Eq(nameof(item.Record), "test/record/with/stats"))
                        .UniqueResult<DatabaseItem>();

                    loaded.Stats.Should().Have.Count.EqualTo(1);
                    loaded.Stats.First().Stat.Should().Be.EqualTo("TestStat");
                }
            }


        }
        [TestMethod]
        public void CanSaveItemWithStats() {
            var item = new DatabaseItem {
                Name = "Magic item",
                Record = "test/record/with/stats"
            };

            List<DatabaseItemStat> stats = new List<DatabaseItemStat>();
            item.Stats = stats;

            stats.Add(new DatabaseItemStat {
                //     Parent = item,
                Stat = "TestStat",
                TextValue = "Value"
            });

            var s = new HashSet<DatabaseItem>();
            s.Add(item);
            _dao.Save(s);

            item.Id.Should().Be.GreaterThan(0);
            item.Stats.FirstOrDefault().Id.Should().Be.GreaterThan(0);



            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    DatabaseItem loaded = session
                        .CreateCriteria<DatabaseItem>()
                        .Add(Restrictions.Eq(nameof(item.Record), "test/record/with/stats"))
                        .UniqueResult<DatabaseItem>();

                    loaded.Stats.Should().Have.Count.EqualTo(1);
                    loaded.Stats.First().Stat.Should().Be.EqualTo("TestStat");
                }
            }


        }
    }
}

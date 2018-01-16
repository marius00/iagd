using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Model;
using log4net;
using log4net.Repository.Hierarchy;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IAGrim.Database.DAO {
    class ItemTagDaoImpl : BaseDao<ItemTag>, IItemTagDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTagDaoImpl));

        public ItemTagDaoImpl(ISessionCreator sessionCreator) : base(sessionCreator) {
        }

        public void Save(ICollection<ItemTag> items, ProgressTracker tracker) {
            int commitSize = items.Count / 10;
            tracker.MaxValue = items.Count + commitSize;
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery("DELETE FROM ItemTag").ExecuteUpdate();
                    foreach (ItemTag item in items) {
                        session.Insert(item);
                        tracker.Increment();
                    }
                    transaction.Commit();

                    // A bit of 'fake pending progress' for commit
                    for (int i = 0; i < commitSize; i++) {
                        tracker.Increment();
                    }
                }
            }

            Logger.InfoFormat("Stored {0} item tags to database..", items.Count);
        }


        public IList<ItemTag> GetClassItemTags() {

            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<ItemTag>()
                        .Add(Restrictions.Like(nameof(ItemTag.Tag), "tagSkillClassName%"))
                        .List<ItemTag>()
                        .Select(m => new Database.ItemTag { Name = m.Name, Tag = m.Tag.Replace("tagSkillClassName", "class") })
                        .ToList();
                }
            }
        }
        public IList<ItemTag> GetValidClassItemTags() {

            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateSQLQuery("SELECT * FROM ItemTag WHERE (Tag LIKE 'tagSkillClassName%' OR Tag LIKE 'tag%Class%SkillName00A') AND LENGTH(Name) > 1")
                        .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ItemTag)))
                        .List<ItemTag>()
                        .Select(m => new ItemTag {
                            Name = m.Name,
                            Tag = m.Tag.Replace("tagSkillClassName", "class")
                                .Replace("tagGDX1Class07SkillName00A", "class07")
                                .Replace("tagGDX1Class08SkillName00A", "class08")
                        })
                        .ToList();
                }
            }
        }
    }
}

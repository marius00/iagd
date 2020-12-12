using System;
using System.Collections.Generic;
using System.Linq;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Model;
using log4net;
using MoreLinq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IAGrim.Database.DAO {
    class ItemTagDaoImpl : BaseDao<ItemTag>, IItemTagDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTagDaoImpl));

        public ItemTagDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {
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
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<ItemTag>()
                        .Add(Restrictions.Like(nameof(ItemTag.Tag), "tagSkillClassName%"))
                        .List<ItemTag>()
                        .Select(m => new ItemTag { Name = m.Name, Tag = m.Tag.Replace("tagSkillClassName", "class") })
                        .ToList();
                }
            }
        }

        public ISet<ItemTag> GetValidClassItemTags() {
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery("SELECT * FROM ItemTag WHERE (Tag LIKE 'tagSkillClassName%' OR Tag LIKE 'tag%Class%SkillName00A') AND LENGTH(Name) > 1")
                        .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ItemTag)))
                        .List<ItemTag>()
                        .Select(m => new ItemTag {
                            Name = m.Name,
                            Tag = m.Tag.Replace("tagSkillClassName", "class")
                                .Replace("tagGDX1Class07SkillName00A", "class07")
                                .Replace("tagGDX1Class08SkillName00A", "class08")
                                .Replace("tagGDX2Class09SkillName00A", "class09") // TODO: A regex or similar to auto detect new classes?
                        })
                        .ToHashSet();
                }
            }
        }
    }
}

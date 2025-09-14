using System.Text.RegularExpressions;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Model;
using log4net;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IAGrim.Database.DAO {
    class ItemTagDaoImpl : BaseDao<ItemTag>, IItemTagDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemTagDaoImpl));

        public ItemTagDaoImpl(SessionFactory sessionCreator) : base(sessionCreator) { }

        public void Save(ICollection<ItemTag> items, ProgressTracker tracker) {
            var commitSize = items.Count / 10;
            tracker.MaxValue = items.Count + commitSize;

            using (var session = SessionCreator.OpenStatelessSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.CreateQuery("DELETE FROM ItemTag").ExecuteUpdate();

                    foreach (var item in items) {
                        session.Insert(item);
                        tracker.Increment();
                    }

                    transaction.Commit();

                    // A bit of 'fake pending progress' for commit
                    for (var i = 0; i < commitSize; i++) {
                        tracker.Increment();
                    }
                }
            }

            Logger.InfoFormat("Stored {0} item tags to database..", items.Count);
        }

        public IList<ItemTag> GetClassItemTags() {
            const string namePattern = @"\[ms\](.*)\[fs\](.*)";

            using (var session = SessionCreator.OpenStatelessSession()) {
                return session.CreateCriteria<ItemTag>()
                    .Add(Restrictions.Like(nameof(ItemTag.Tag), "tagSkillClassName%"))
                    .List<ItemTag>()
                    .Select(m => new ItemTag {
                        Name = Regex.IsMatch(m.Name, namePattern)
                            ? Regex.Replace(m.Name, namePattern, "$1/$2")
                            : m.Name,
                        Tag = m.Tag.Replace("tagSkillClassName", "class")
                    })
                    .ToList();
            }
        }

        public ISet<ItemTag> GetValidClassItemTags() {
            const string gdxClassesPattern = @"tagGDX\dClass(\d+)SkillName00A";
            const string namePattern = @"\[ms\](.*)\[fs\](.*)";

            using (var session = SessionCreator.OpenStatelessSession()) {
                return session
                    .CreateSQLQuery(
                        "SELECT * FROM ItemTag WHERE (Tag LIKE 'tagSkillClassName%' OR Tag LIKE 'tag%Class%SkillName00A') AND LENGTH(Name) > 1")
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ItemTag)))
                    .List<ItemTag>()
                    .Select(m => new ItemTag {
                        Name = Regex.IsMatch(m.Name, namePattern)
                            ? Regex.Replace(m.Name, namePattern, "$1/$2")
                            : m.Name,
                        Tag = Regex.IsMatch(m.Tag, gdxClassesPattern)
                            ? Regex.Replace(m.Tag, gdxClassesPattern, "class$1")
                            : m.Tag.Replace("tagSkillClassName", "class")
                    })
                    .ToHashSet();
            }
        }
    }
}
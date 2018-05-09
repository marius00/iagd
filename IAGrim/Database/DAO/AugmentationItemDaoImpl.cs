using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using log4net;
using NHibernate;
using NHibernate.Criterion;

namespace IAGrim.Database.DAO {
    class AugmentationItemDaoImpl : BaseDao<AugmentationItem>, IAugmentationItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AugmentationItemDaoImpl));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        public AugmentationItemDaoImpl(ISessionCreator sessionCreator, IDatabaseItemStatDao databaseItemStatDao) : base(sessionCreator) {
            _databaseItemStatDao = databaseItemStatDao;
        }

        public void UpdateState() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    Populate(session);

                    IList<AugmentationItem> items = session.CreateCriteria<AugmentationItem>().List<AugmentationItem>();
                    var stats = _databaseItemStatDao.GetStats(session, StatFetch.AugmentItems);

                    foreach (var item in items) {
                        var rarity = ItemOperationsUtility.GetRarityForRecord(stats, item.BaseRecord);
                        var minimumLevel = ItemOperationsUtility.GetMinimumLevelForRecord(stats, item.BaseRecord);
                        var name = ItemOperationsUtility.GetItemName(session, stats, item);

                        item.Rarity = rarity;
                        item.MinimumLevel = minimumLevel;
                        item.Name = name;
                        session.Update(item);
                    }

                    transaction.Commit();
                }
            }
        }

        public IList<AugmentationItem> Search(Search query) {

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    ICriteria criterias = session.CreateCriteria<AugmentationItem>();


                    if (!string.IsNullOrEmpty(query.Wildcard)) {
                        criterias.Add(Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItem>()
                            .Add(Restrictions.InsensitiveLike("Name", string.Format("%{0}%", query.Wildcard.Replace(' ', '%'))))
                            .SetProjection(Projections.Property("Record"))));
                    }

                    DatabaseItemDaoImpl.AddItemSearchCriterias(criterias, query);
                    IList<AugmentationItem> items = criterias.List<AugmentationItem>();

                    return items;
                }
            }
        }

        private void Populate(ISession session) {
            string sql = @"
                    INSERT OR IGNORE INTO AugmentationItem (id_databaseitem, baserecord, name, rarity, minimumlevel) 
                    select db.id_databaseitem, baserecord, name, null, null 
                    from DatabaseItem_v2 db
                    where db.id_databaseitem in (select s.id_databaseitem from DatabaseItemStat_v2 s where stat = 'Class' and textvalue = 'ItemEnchantment') 
                    order by name";

            session.CreateSQLQuery(sql).ExecuteUpdate();
        }
    }
}

using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using IAGrim.Database.Model;

namespace IAGrim.Database {
    /// <summary>
    /// List of items in game, and which the player already owns.
    /// </summary>
    public class ItemCollectionDaoImpl : BaseDao<CollectionItem>, IItemCollectionDao {
        public ItemCollectionDaoImpl(ISessionCreator sessionCreator) : base(sessionCreator) {
        }

        public IList<CollectionItem> GetItemCollection() {
            const string sql = @"
            select baserecord as BaseRecord,
            name as Name, 
            (select count(*) from PlayerItem P where P.baserecord = item.baserecord) as NumOwned,
            (select textvalue from DatabaseItemStat_v2 s2 where s2.id_databaseitem = s.id_databaseitem and s2.stat like '%itmap%' limit 1) as Icon
            from DatabaseItemStat_v2 s, DatabaseItem_v2 item 
            where s.stat = 'itemClassification' 
            and s.textvalue = 'Legendary'
            and item.id_databaseitem = s.id_databaseitem
            and baserecord not like '%/crafting/%'
            and name is not null
            and name != ''
            order by name asc
";

            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<CollectionItem>())
                        .List<CollectionItem>();
                }
            }
        }

    }
}

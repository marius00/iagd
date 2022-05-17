using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;

namespace IAGrim.Database {
    /// <summary>
    /// List of items in game, and which the player already owns.
    /// </summary>
    public class ItemCollectionDaoImpl : BaseDao<CollectionItem>, IItemCollectionDao {
        public ItemCollectionDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {
        }

        public IList<CollectionItem> GetItemCollection(ItemSearchRequest query) {
            // TODO: Possible to merge NumOwnedSC and NumOwnedHc via a join? Avoid two heavy subqueries.
            List<string> sql = new List<string>();
            sql.Add(@"
            select baserecord as BaseRecord,
            name as Name, 
            (select count(*) from PlayerItem P where P.baserecord = item.baserecord AND NOT ishardcore) as NumOwnedSc,
            (select count(*) from PlayerItem P where P.baserecord = item.baserecord AND ishardcore) as NumOwnedHc,
            (select textvalue from DatabaseItemStat_v2 s2 where s2.id_databaseitem = s.id_databaseitem and s2.stat like '%itmap%' limit 1) as Icon,
			s.textvalue as Quality
            from DatabaseItemStat_v2 s, DatabaseItem_v2 item 
            where s.stat = 'itemClassification' 
            and (s.textvalue = 'Legendary' OR s.textvalue = 'Epic')
            and item.id_databaseitem = s.id_databaseitem
            and baserecord not like '%/crafting/%'
            and name is not null
            and name != ''
");

            if (query.Slot?.Length > 0) {
                sql.Add(@"			
                    AND exists (
                        select id_databaseitem from databaseitemstat_v2 dbs 
                        WHERE stat = 'Class' 
                        AND TextValue in ( :class ) 
                        AND item.id_databaseitem = dbs.id_databaseitem
                    )");
            }

            if (!string.IsNullOrEmpty(query.Wildcard)) {
                sql.Add("AND name LIKE :name");
            }


            sql.Add(@"order by name asc");


            
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    var q = session.CreateSQLQuery(string.Join(" ", sql));

                    if (query.Slot?.Length > 0) {
                        q.SetParameterList("class", query.Slot);
                    }
                    if (!string.IsNullOrEmpty(query.Wildcard)) {
                        q.SetParameter("name", $"%{query.Wildcard.ToLower()}%");
                    }



                    return q.SetResultTransformer(Transformers.AliasToBean<CollectionItem>()).List<CollectionItem>();
                }
            }
        }

        /// <summary>
        /// Returns a table of item stats
        /// How many of your items are purple, blue, how many are feet, waist, etc..
        /// </summary>
        public IList<CollectionItemAggregateRow> GetItemAggregateStats() {
            const string sql = @"select count(pi.id) as Num, pi.rarity || (case when PrefixRarity <= 1 then '' else PrefixRarity end) as Quality, s.textvalue as Slot from playeritem pi, DatabaseItem_v2 dbi, DatabaseItemStat_v2 s
where pi.baserecord = dbi.baserecord
and dbi.id_databaseitem = s.id_databaseitem
and s.stat = 'Class'
and rarity != 'White'
and rarity != 'Yellow'
and rarity != 'Unknown'

group by rarity, s.textvalue
order by textvalue, rarity";


            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<CollectionItemAggregateRow>())
                        .List<CollectionItemAggregateRow>();
                }
            }
        }

    }
}

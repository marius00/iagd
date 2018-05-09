using IAGrim.Database;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.Transform;
using IAGrim.Services.Dto;
using log4net;
using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Model;
using MoreLinq;

namespace IAGrim.Services {
    class ItemStatService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemStatService));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IItemSkillDao _itemSkillDao;
        private bool _displaySkills => Properties.Settings.Default.DisplaySkills;
        private readonly Dictionary<string, ISet<DBSTatRow>> _xpacSkills;




        public ItemStatService(
            IDatabaseItemStatDao databaseItemStatDao, 
            IItemSkillDao itemSkillDao
        ) {
            this._databaseItemStatDao = databaseItemStatDao;
            this._itemSkillDao = itemSkillDao;
            _xpacSkills = _databaseItemStatDao.GetExpacSkillModifierSkills();
        }



        private static List<string> GetRecordsForItem(BaseItem item) {
            List<string> records = new List<string>();
            if (!string.IsNullOrEmpty(item.BaseRecord)) {
                records.Add(item.BaseRecord);
            }
            if (!string.IsNullOrEmpty(item.PrefixRecord)) {
                records.Add(item.PrefixRecord);
            }
            if (!string.IsNullOrEmpty(item.SuffixRecord)) {
                records.Add(item.SuffixRecord);
            }
            if (!string.IsNullOrEmpty(item.MateriaRecord)) {
                records.Add(item.MateriaRecord);
            }
            if (!string.IsNullOrEmpty(item.PetRecord)) {
                records.Add(item.PetRecord);
            }

            return records;
        }


        private static List<string> GetRecordsForItems(List<PlayerHeldItem> items) {
            return items.Select(m => m.BaseRecord).Distinct().ToList();
        }

        private bool IsPlayerItem(PlayerHeldItem item) {
            return string.IsNullOrEmpty(item.Stash) && !item.IsRecipe;
        }

        private bool IsBuddyItem(PlayerHeldItem item) {
            return !string.IsNullOrEmpty(item.Stash) && !item.IsRecipe;
        }

        private List<PlayerItem> GetPlayerItems(List<PlayerHeldItem> items) {
            var result = new List<PlayerItem>();
            foreach (PlayerHeldItem item in items.Where(IsPlayerItem)) {
                if (item is PlayerItem pi) {
                    result.Add(pi);
                }
            }

            return result;
        }

        private List<BuddyItem> GetBuddyItems(List<PlayerHeldItem> items) {
            var result = new List<BuddyItem>();
            items.Where(IsBuddyItem).ForEach(item => result.Add(item as BuddyItem));
            return result;
        }

        private void ApplyStatsToAugmentations(List<PlayerHeldItem> items) {
            ApplyStatsToDbItems(items, StatFetch.AugmentItems);
        }

        private void ApplyStatsToRecipeItems(List<PlayerHeldItem> items) {
            ApplyStatsToDbItems(items, StatFetch.RecipeItems);
        }

        private void ApplyStatsToDbItems(List<PlayerHeldItem> items, StatFetch type) {

            var records = GetRecordsForItems(items);
            Dictionary<string, List<DBSTatRow>> statMap = _databaseItemStatDao.GetStats(records, type);

            foreach (PlayerHeldItem phi in items) {
                List<DBSTatRow> stats = new List<DBSTatRow>();
                if (statMap.ContainsKey(phi.BaseRecord))
                    stats.AddRange(Filter(statMap[phi.BaseRecord]));

                var statsWithText = Filter(stats.Where(m => !string.IsNullOrEmpty(m.TextValue)));
                List<DBSTatRow> statsWithNumerics = stats.Where(m => string.IsNullOrEmpty(m.TextValue))
                    .GroupBy(r => r.Stat)
                    .Select(g => new DBSTatRow {
                        Record = g.FirstOrDefault()?.Record,
                        TextValue = g.FirstOrDefault()?.TextValue,
                        Stat = g.FirstOrDefault()?.Stat,
                        Value = g.Sum(v => v.Value)
                    })
                    .ToList();

                statsWithNumerics.AddRange(statsWithText);

                phi.Tags = new HashSet<DBSTatRow>(statsWithNumerics);
            }

            Logger.Debug($"Applied stats to {items.Count()} items");
        }

        private void ApplyMythicalBonuses(List<PlayerHeldItem> items) {
            var itemsWithXpacStat = items.Where(m => m.Tags.Any(s => s.Stat == "modifiedSkillName1"));
            foreach (var item in itemsWithXpacStat) {
                var affectedSkill = item.Tags.FirstOrDefault(m => m.Stat == "modifiedSkillName1");
                var recordForStats = item.Tags.FirstOrDefault(m => m.Stat == "modifierSkillName1")?.TextValue;

                if (recordForStats == null || !_xpacSkills.ContainsKey(recordForStats)) {
                    Logger.Warn($"Could not find stats for the skill {recordForStats}");
                    continue;
                }

                var name = affectedSkill?.TextValue;
                if (!string.IsNullOrEmpty(name)) {
                    name = _databaseItemStatDao.GetSkillName(name);
                }
                item.ModifiedSkills.Add(new SkillModifierStat {
                    Tags = _xpacSkills[recordForStats],
                    Name = name
                });
            }
        }

        public void ApplyStats(List<PlayerHeldItem> items) {
            if (items.Count > 0)
            {
                Logger.Debug($"Applying stats to {items.Count()} items");

                var playerItems = GetPlayerItems(items);
                if (playerItems.Count > 0)
                {
                    ApplyStatsToPlayerItems(playerItems);
                }

                var buddyItems = GetBuddyItems(items);
                if (buddyItems.Count > 0) {
                    ApplyStatsToBuddyItems(buddyItems);
                }

                // Augments sold by vendors, which the player doesn't necessarily own
                var augmentItems = items.Where(m => (m as AugmentationItem) != null).ToList();
                if (augmentItems.Count > 0) {
                    ApplyStatsToAugmentations(augmentItems);
                }

                var remaining = items.Where(m => !IsPlayerItem(m) && !IsBuddyItem(m) && !augmentItems.Contains(m)).ToList();
                if (remaining.Count > 0) {
                    ApplyStatsToRecipeItems(remaining);
                }

                ApplyMythicalBonuses(items);
            }
        }

        private void ApplySkills(List<PlayerItem> items) {
            Logger.Debug($"Applying skills to items");
            var skills = _itemSkillDao.List();
            foreach (var item in items) {
                item.Skill = skills.FirstOrDefault(skill => skill.PlayerItemRecord == item.BaseRecord);
            }


            var itemsWithSkills = items.Where(m => m.Skill != null).Select(m => m.Skill.StatsId).ToList();
            if (itemsWithSkills.Count > 0) {
                Logger.Debug($"Applying stats to skills for {itemsWithSkills.Count} items");
                Dictionary<long, List<DBSTatRow>> statMap =
                    _databaseItemStatDao.GetStats(itemsWithSkills, StatFetch.Skills);

                foreach (var item in items.Where(m => m.Skill != null)) {
                    if (statMap.ContainsKey(item.Skill.StatsId)) {
                        item.Skill.Tags = process(statMap[item.Skill.StatsId]);
                    }
                    else {
                        Logger.Warn($"Could not map skill with stat id {item.Skill.StatsId} for item {item.Name}");
                    }
                }
            }
            else {
                Logger.Debug("No items with skills, skipping stat application");
            }

            Logger.Debug("Skills applied");
        }

        public void ApplyStatsToBuddyItems(List<BuddyItem> items) {
            if (items.Count > 0) {
                Logger.Debug($"Applying stats to {items.Count} items");
                var records = items.SelectMany(GetRecordsForItem).Distinct();
                Dictionary<string, List<DBSTatRow>> statMap = _databaseItemStatDao.GetStats(records, StatFetch.BuddyItems);
                
                // TODO: Add skill support?
                /*
                if (_displaySkills)
                    ApplySkills(items);
                    */

                foreach (var pi in items) {
                    List<DBSTatRow> stats = new List<DBSTatRow>();
                    if (statMap.ContainsKey(pi.BaseRecord))
                        stats.AddRange(Filter(statMap[pi.BaseRecord]));

                    if (!string.IsNullOrEmpty(pi.PrefixRecord) && statMap.ContainsKey(pi.PrefixRecord))
                        stats.AddRange(Filter(statMap[pi.PrefixRecord]));

                    if (!string.IsNullOrEmpty(pi.SuffixRecord) && statMap.ContainsKey(pi.SuffixRecord))
                        stats.AddRange(Filter(statMap[pi.SuffixRecord]));

                    if (!string.IsNullOrEmpty(pi.ModifierRecord) && statMap.ContainsKey(pi.ModifierRecord))
                        stats.AddRange(Filter(statMap[pi.ModifierRecord]));

                    if (!string.IsNullOrEmpty(pi.PetRecord) && statMap.ContainsKey(pi.PetRecord))
                        stats.AddRange(Filter(statMap[pi.PetRecord]));


                    pi.Tags = process(stats);
                }

                Logger.Debug($"Applied stats to {items.Count} items");
            }
        }

        public void ApplyStatsToPlayerItems(List<PlayerItem> items) {
            if (items.Count > 0) {
                Logger.Debug($"Applying stats to {items.Count} items");
                var records = items.SelectMany(GetRecordsForItem).Distinct();
                Dictionary<string, List<DBSTatRow>> statMap = _databaseItemStatDao.GetStats(records, StatFetch.PlayerItems);

                if (_displaySkills) {
                    ApplySkills(items);
                }

                foreach (PlayerItem pi in items) {
                    List<DBSTatRow> stats = new List<DBSTatRow>();
                    if (statMap.ContainsKey(pi.BaseRecord))
                        stats.AddRange(Filter(statMap[pi.BaseRecord]));

                    if (!string.IsNullOrEmpty(pi.PrefixRecord) && statMap.ContainsKey(pi.PrefixRecord))
                        stats.AddRange(Filter(statMap[pi.PrefixRecord]));

                    if (!string.IsNullOrEmpty(pi.SuffixRecord) && statMap.ContainsKey(pi.SuffixRecord))
                        stats.AddRange(Filter(statMap[pi.SuffixRecord]));

                    if (!string.IsNullOrEmpty(pi.ModifierRecord) && statMap.ContainsKey(pi.ModifierRecord))
                        stats.AddRange(Filter(statMap[pi.ModifierRecord]));

                    if (!string.IsNullOrEmpty(pi.PetRecord) && statMap.ContainsKey(pi.PetRecord))
                        stats.AddRange(Filter(statMap[pi.PetRecord]));
                    // TODO: Don't do this, use PlayerItemRecords.. somehow.. those contain pet bonuses

                    pi.Tags = process(stats);
                }

                Logger.Debug($"Applied stats to {items.Count} items");
            }
        }


        private HashSet<DBSTatRow> process(List<DBSTatRow> stats) {

            var statsWithText = Filter(stats.Where(m => !string.IsNullOrEmpty(m.TextValue)));
            List<DBSTatRow> statsWithNumerics = stats.Where(m => string.IsNullOrEmpty(m.TextValue))
                .GroupBy(r => r.Stat)
                .Select(g => new DBSTatRow {
                    Record = g.FirstOrDefault().Record,
                    TextValue = g.FirstOrDefault().TextValue,
                    Stat = g.FirstOrDefault().Stat,
                    Value = g.Sum(v => v.Value),
                })
                .ToList();

            statsWithNumerics.AddRange(statsWithText);
            return new HashSet<DBSTatRow>(statsWithNumerics);
        }

        private IEnumerable<DBSTatRow> Filter(IEnumerable<DBSTatRow> stats) {
            return stats.GroupBy(r => r.Stat)
                    .Select(g => g.OrderByDescending(m => {
                        return m.Value;
                    }).First());
        }
        
    }
}

using IAGrim.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.Services.Dto;
using log4net;
using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Model;
using IAGrim.Parsers.Arz;
using IAGrim.Settings;

namespace IAGrim.Services {
    public class ItemStatService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemStatService));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IItemSkillDao _itemSkillDao;
        private bool HideSkills => _settings.GetPersistent().HideSkills;

        private readonly SettingsService _settings;


        public ItemStatService(
            IDatabaseItemStatDao databaseItemStatDao,
            IItemSkillDao itemSkillDao,
            SettingsService settings) {
            this._databaseItemStatDao = databaseItemStatDao;
            this._itemSkillDao = itemSkillDao;
            _settings = settings;
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

            records.AddRange(item.PetRecords);

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
            return items.Where(IsBuddyItem).Select(item => item as BuddyItem).ToList();
        }


        /// <summary>
        /// This prepares the "Tags" list, which is used for icons and determining item type for tooltips
        /// </summary>
        /// <param name="itemSource"></param>
        public void ApplyStats(IEnumerable<PlayerHeldItem> itemSource) {
            var items = itemSource.ToList();
            if (items.Count > 0) {
                Logger.Debug($"Applying stats to {items.Count()} items");

                var playerItems = GetPlayerItems(items);
                if (playerItems.Count > 0) {
                    ApplyStatsToPlayerItems(playerItems);
                }

                var buddyItems = GetBuddyItems(items);
                if (buddyItems.Count > 0) {
                    ApplyStatsToBuddyItems(buddyItems);
                }
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
                Dictionary<long, List<DBStatRow>> statMap =
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
                Dictionary<string, List<DBStatRow>> statMap =
                    _databaseItemStatDao.GetStats(records, StatFetch.BuddyItems);

                foreach (var pi in items) {
                    List<DBStatRow> stats = new List<DBStatRow>();
                    if (statMap.ContainsKey(pi.BaseRecord))
                        stats.AddRange(Filter(statMap[pi.BaseRecord]));

                    if (!string.IsNullOrEmpty(pi.PrefixRecord) && statMap.ContainsKey(pi.PrefixRecord))
                        stats.AddRange(Filter(statMap[pi.PrefixRecord]));

                    if (!string.IsNullOrEmpty(pi.SuffixRecord) && statMap.ContainsKey(pi.SuffixRecord))
                        stats.AddRange(Filter(statMap[pi.SuffixRecord]));

                    if (!string.IsNullOrEmpty(pi.ModifierRecord) && statMap.ContainsKey(pi.ModifierRecord))
                        stats.AddRange(Filter(statMap[pi.ModifierRecord]));

                    foreach (var petRecord in pi.PetRecords) {
                        if (!string.IsNullOrEmpty(petRecord) && statMap.ContainsKey(petRecord))
                            stats.AddRange(Filter(statMap[petRecord]));
                    }


                    pi.Tags = process(stats);
                }

                Logger.Debug($"Applied stats to {items.Count} items");
            }
        }

        public void ApplyStatsToPlayerItems(List<PlayerItem> items) {
            if (items.Count > 0) {
                Logger.Debug($"Applying stats to {items.Count} items");
                var records = items.SelectMany(GetRecordsForItem).Distinct();
                Dictionary<string, List<DBStatRow>> statMap =
                    _databaseItemStatDao.GetStats(records, StatFetch.PlayerItems);

                if (!HideSkills) {
                    ApplySkills(items);
                }

                foreach (PlayerItem pi in items) {
                    List<DBStatRow> stats = new List<DBStatRow>();
                    if (statMap.ContainsKey(pi.BaseRecord))
                        stats.AddRange(Filter(statMap[pi.BaseRecord]));

                    if (!string.IsNullOrEmpty(pi.PrefixRecord) && statMap.ContainsKey(pi.PrefixRecord))
                        stats.AddRange(Filter(statMap[pi.PrefixRecord]));

                    if (!string.IsNullOrEmpty(pi.SuffixRecord) && statMap.ContainsKey(pi.SuffixRecord))
                        stats.AddRange(Filter(statMap[pi.SuffixRecord]));

                    if (!string.IsNullOrEmpty(pi.ModifierRecord) && statMap.ContainsKey(pi.ModifierRecord))
                        stats.AddRange(Filter(statMap[pi.ModifierRecord]));

                    foreach (var petRecord in pi.PetRecords) {
                        if (!string.IsNullOrEmpty(petRecord) && statMap.ContainsKey(petRecord)) {
                            var petRecords = Filter(statMap[petRecord]);
                            stats.AddRange(petRecords);
                        }
                    }
                    // TODO: Don't do this, use PlayerItemRecords.. somehow.. those contain pet bonuses

                    pi.Tags = process(stats);
                }


                Logger.Debug($"Applied stats to {items.Count} items");
            }
        }


        private HashSet<DBStatRow> process(List<DBStatRow> stats) {
            var statsWithText = stats.Where(m => !string.IsNullOrEmpty(m.TextValue));
            List<DBStatRow> statsWithNumerics = stats.Where(m => string.IsNullOrEmpty(m.TextValue))
                .GroupBy(r => r.Stat)
                .Select(g => new DBStatRow {
                    Record = g.FirstOrDefault().Record,
                    TextValue = g.FirstOrDefault().TextValue,
                    Stat = g.FirstOrDefault().Stat,
                    Value = g.Sum(v => v.Value),
                })
                .ToList();

            statsWithNumerics.AddRange(statsWithText);
            return new HashSet<DBStatRow>(statsWithNumerics);
        }

        private IEnumerable<DBStatRow> Filter(IEnumerable<DBStatRow> stats) {
            return stats.GroupBy(r => r.Stat)
                .Select(g => g.OrderByDescending(m => { return m.Value; }).First());
        }
    }
}
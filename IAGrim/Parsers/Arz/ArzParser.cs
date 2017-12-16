using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Arc;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Parsers.Arz {
    public class ArzParser : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ArzParser));
        private readonly IDatabaseItemDao _databaseItemDao;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IDatabaseSettingDao _databaseSettingDao;
        private readonly IItemSkillDao _itemSkillDao;
        private readonly List<DatabaseItem> _skills = new List<DatabaseItem>();
        private ISet<ItemTag> _tags;

        public ArzParser(
            IDatabaseItemDao databaseItemDao,
            IDatabaseItemStatDao databaseItemStatDao,
            IDatabaseSettingDao databaseSettingDao,
            IItemSkillDao itemSkillDao
        ) {
            _databaseItemDao = databaseItemDao;
            _databaseItemStatDao = databaseItemStatDao;
            _databaseSettingDao = databaseSettingDao;
            _itemSkillDao = itemSkillDao;
        }

        // LocalizationFile

        private Dictionary<string, string> LoadTags(string arcTagfile, string localizationFile, bool expansionOnlyMod) {
            // Process and load the _tags Load the _tags
            Dictionary<string, string> mappedTags;

            bool isTagfileLocked = IOHelper.IsFileLocked(new FileInfo(arcTagfile));
            TemporaryCopy arcTagTemp = isTagfileLocked ? new TemporaryCopy(arcTagfile) : null;


            // Load from user localization
            var localizationLoader = new LocalizationLoader();
            if (!string.IsNullOrEmpty(localizationFile) && localizationLoader.Load(localizationFile)) {
                _tags = localizationLoader.GetItemTags();
            }

            // Load from GD
            else {
                List<IItemTag> _tags =
                    Parser.Arz.ArzParser.ParseArcFile(isTagfileLocked ? arcTagTemp.Filename : arcTagfile);
                this._tags = new HashSet<ItemTag>(_tags.Select(m => new ItemTag {
                        Name = m.Name,
                        Tag = m.Tag
                    }).ToArray()
                );
            }

            bool saveOrUpdate = expansionOnlyMod;
            if (saveOrUpdate) {
                _databaseItemDao.SaveOrUpdate(_tags);
            }
            else {
                _databaseItemDao.Save(_tags);
            }

            // Fallback: Just use whatever names we got stored in db
            if (_tags == null || _tags.Count == 0) {
                Logger.Info(
                    "Using cached tag dictionary instead, this may or may not be compatible with the supplied mod.");
                Logger.Info("If this is causing any issues, switch to vanilla then back to the mod.");
                mappedTags = _databaseItemDao.GetTagDictionary();
            }
            else {
                mappedTags = _tags.ToDictionary(item => item.Tag, item => item.Name);
                var dbTags = _databaseItemDao.GetTagDictionary();
                foreach (var key in dbTags.Keys.Where(key => !mappedTags.ContainsKey(key))) {
                    mappedTags[key] = dbTags[key];
                }
            }

            return mappedTags;
        }


        /// <summary>
        ///     Locate the ARZ file for a given mod or GD install
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static string FindArzFile(string parent) {
            if (!Directory.Exists(Path.Combine(parent, "database"))) {
                Logger.WarnFormat("Could not find the \"database\" directory at \"{0}\"", parent);
            }
            else {
                foreach (var file in Directory.EnumerateFiles(Path.Combine(parent, "database"), "*.arz")) {
                    return file;
                }
            }

            return string.Empty;
        }


        /// <summary>
        ///     Locate a given Arc file for a given mod or GD install
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="arc"></param>
        /// <returns></returns>
        private static string FindArcFile(string parent, string arc) {
            if (File.Exists(Path.Combine(parent, "resources", arc)))
                return Path.Combine(parent, "resources", arc);
            return string.Empty;
        }


        public bool NeedUpdate(string grimdawnLocation) {
            string databaseFile = FindArzFile(grimdawnLocation);
            if (!string.IsNullOrEmpty(databaseFile)) {
                long lastModified = File.GetLastWriteTime(databaseFile).Ticks;
                if (_databaseSettingDao.GetLastDatabaseUpdate() < lastModified) {
                    Logger.Debug($"A new database parse is required, the file \"{databaseFile}\" has been modified");
                    return true;
                }

                var expansionLocation = Path.Combine(grimdawnLocation, "gdx1");
                if (Directory.Exists(expansionLocation))
                    return NeedUpdate(expansionLocation);
            }
            else {
                Logger.WarnFormat("Could not locate database at \"{0}\"", grimdawnLocation);
            }
            return false;
        }

        /// <summary>
        ///     Just reset the _tags
        ///     Usually after reverting from a language pack
        /// </summary>
        /// <param name="grimdawnLocation"></param>
        /// <param name="expansionOnlyMod"></param>
        public void LoadArcTags(string grimdawnLocation, bool expansionOnlyMod) {
            string arcTagfile = FindArcFile(grimdawnLocation, "text_en.arc");
            if (string.IsNullOrEmpty(arcTagfile)) {
                Logger.FatalFormat("Unable to parse the Grim Dawn _tags, could not find the relevant files at {0}",
                    grimdawnLocation);
                return;
            }

            LoadTags(arcTagfile, string.Empty, expansionOnlyMod);
        }

        private DatabaseItemStat map(IItemStat stat) {
            return new DatabaseItemStat {
                Stat = stat.Stat,
                TextValue = stat.TextValue,
                Value = stat.Value
            };
        }

        public static void LoadIconsOnly(string grimdawnLocation) {
            Logger.Debug("Icon loading requested");
            {
                var arcItemsFile = FindArcFile(grimdawnLocation, "items.arc");
                if (!string.IsNullOrEmpty(arcItemsFile)) {
                    Logger.Debug($"Loading vanilla icons from {arcItemsFile}");
                    LoadIcons(arcItemsFile);
                }
                else {
                    Logger.Warn("Could not find the vanilla icons, skipping.");
                }
            }

            var expansionFolder = Path.Combine(grimdawnLocation, "gdx1");
            if (Directory.Exists(expansionFolder)) {
                var arcItemsFile = FindArcFile(expansionFolder, "items.arc");
                if (!string.IsNullOrEmpty(arcItemsFile)) {
                    Logger.Debug($"Loading expansion icons from {arcItemsFile}");
                    LoadIcons(arcItemsFile);
                }
                else {
                    Logger.Warn("Could not find the expansion, skipping.");
                }
            }

            var crucibleFolder = Path.Combine(grimdawnLocation, "mods", "survivalmode");
            if (Directory.Exists(crucibleFolder)) {
                var arcItemsFile = FindArcFile(crucibleFolder, "items.arc");
                if (!string.IsNullOrEmpty(arcItemsFile)) {
                    Logger.Debug($"Loading \"The Crucible\" icons from {arcItemsFile}");
                    LoadIcons(arcItemsFile);
                }
                else {
                    Logger.Warn("Could not find \"The crucible\", skipping.");
                }
            }
        }

        /// <summary>
        ///     Parses the data from Grim Dawn (arz and arc files)
        ///     If the supplied location is a mod, supply whetever its an addative or a replacement for the original database.
        ///     Eg, should it clear the existing database upon insert, or add/replace?
        ///     The prior is faster, but will not work for mods with a partial database.
        /// </summary>
        /// <param name="grimdawnLocation"></param>
        /// <param name="localizationFile"></param>
        /// <param name="expansionOnlyMod">Whetever this mod only expands upon EXISTING content</param>
        public void LoadArzDb(string grimdawnLocation, string localizationFile, bool expansionOnlyMod) {
            try {
                // Init & abort if not needed
                Logger.InfoFormat("Parse Arz/Arc, Location: \"{0}\", Additive: {1}, ClearFix: {2}", grimdawnLocation,
                    expansionOnlyMod, !expansionOnlyMod);
                string databaseFile = FindArzFile(grimdawnLocation);
                //string expansionDatabaseFile = FindArzFile(Path.Combine(grimdawnLocation, "gdx1"));

                string arcTagfile = FindArcFile(grimdawnLocation, "text_en.arc");
                var arcItemsFile = FindArcFile(grimdawnLocation, "items.arc");


                if (string.IsNullOrEmpty(arcTagfile) || string.IsNullOrEmpty(arcItemsFile) ||
                    string.IsNullOrEmpty(databaseFile)) {
                    Logger.FatalFormat(
                        "Unable to parse the Grim Dawn database, could not find the relevant files at {0}",
                        grimdawnLocation);
                    return;
                }

                //long lastExpansionModified = string.IsNullOrEmpty(expansionDatabaseFile) ? 0 : File.GetLastWriteTime(expansionDatabaseFile).Ticks;
                long lastModified = File.GetLastWriteTime(databaseFile).Ticks;
                Logger.Info("Updating...");
                
                
                var mappedTags = LoadTags(arcTagfile, localizationFile, expansionOnlyMod);


                // Process the items loaded
                var skipLots = true;
                List<IItem> _items = Parser.Arz.ArzParser.LoadItemRecords(databaseFile, skipLots); // TODO

                int x = 9;
                {
                    List<DatabaseItem> items = new List<DatabaseItem>();

                    foreach (IItem item in _items) {
                        items.Add(new DatabaseItem {
                            Record = item.Record,
                            Stats = item.Stats.Select(map).ToList()
                        });
                    }


                    // Map the name using the name _tags
                    MapItemNames(items, mappedTags);
                    RenamePetStats(items);


                    // Add skill accumulation
                    _skills.AddRange(items.Where(m => m.Record.Contains("/skills/")));

                    var specialStats = CreateSpecialRecords(items, mappedTags);
                    Logger.Info($"Mapped {specialStats.Count()} special stats");


                    Logger.Info("Storing items to database..");

                    // Store to DB (cache)
                    if (expansionOnlyMod) {
                        _databaseItemDao.SaveOrUpdate(items);
                    }
                    else {
                        // Deletes all existing data and saves new
                        _databaseItemDao.Save(items);
                        Logger.InfoFormat("Stored {0} items and {1} stats to internal db.", items.Count, -1);
                    }

                    _databaseItemStatDao.Save(specialStats);
                    Logger.Debug("Special stats saved to DB");


                    // Obs: Do this after storing items as the item IDs changes
                    var skillParser = new ComplexItemParser(_items, mappedTags);
                    skillParser.Generate();
                    _itemSkillDao.Save(skillParser.Skills, expansionOnlyMod);
                    _itemSkillDao.Save(skillParser.SkillItemMapping, expansionOnlyMod);


                    _databaseSettingDao.UpdateDatabaseTimestamp(lastModified);

                    LoadIcons(arcItemsFile);

                    _itemSkillDao.EnsureCorrectSkillRecords();
                }
            }
            catch (AggregateException ex) {
                Logger.Warn("System.AggregateException waiting for tasks, throwing first exception.");
                foreach (var inner in ex.InnerExceptions) {
                    Logger.Warn(inner.Message);
                    Logger.Warn(inner.StackTrace);
                    ExceptionReporter.ReportException(inner, "[AggregateException]", true);

                    if (inner.InnerException != null) {
                        Logger.Warn(inner.InnerException.Message);
                        Logger.Warn(inner.InnerException.StackTrace);
                    }
                }


                ex.Handle(x => { throw x; });
            }
        }

        private static void LoadIcons(string arcItemsFile) {
            Logger.Info($"Loading item icons from {arcItemsFile}..");

            bool arcfileLocked = IOHelper.IsFileLocked(new FileInfo(arcItemsFile));
            TemporaryCopy arcTempFile = new TemporaryCopy(arcItemsFile, arcfileLocked);
            string arcItemfile = arcTempFile.Filename;

            if (arcfileLocked) {
                Logger.Info($"The file {arcItemsFile} is currently locked for reading. Perhaps Grim Dawn is running?");
                Logger.Info($"A copy of {arcItemsFile} has been created at {arcItemfile}");
            }
            if (!File.Exists(arcItemfile)) {
                Logger.Warn($"Item icon file \"{arcItemfile}\" could not be located..");
            }

            DDSImageReader.ExtractItemIcons(arcItemfile, GlobalPaths.StorageFolder);
        }

        private void RenamePetStats(List<DatabaseItem> items) {
            Logger.Debug("Detecting records with pet bonus stats..");

            var petRecords = items.SelectMany(m => m.Stats.Where(s => s.Stat == "petBonusName")
                    .Select(s => s.TextValue))
                .ToList(); // ToList for performance reasons

            var petItems = items.Where(m => petRecords.Contains(m.Record)).ToList();
            foreach (var petItem in petItems) {
                var stats = petItem.Stats.Select(s => new DatabaseItemStat {
                    Stat = "pet" + s.Stat,
                    TextValue = s.TextValue,
                    Value = s.Value,
                    Parent = s.Parent
                }).ToList();

                petItem.Stats.Clear();
                petItem.Stats = stats;
            }

            items.RemoveAll(m => petRecords.Contains(m.Record));
            items.AddRange(petItems);

            Logger.Debug($"Classified {petItems.Count()} records as pet stats");
        }


        public static string ExtractClassFromRecord(string record, IEnumerable<DatabaseItem> items) {
            Regex playerClassRx = new Regex(@".*/player(class\d+)/.*");
            // "MasteryEnumeration"	"SkillClass24" => tagSkillClassName24
            var rExSkillClass = playerClassRx.Match(record);
            if (rExSkillClass.Success && rExSkillClass.Groups.Count == 2)
                return rExSkillClass.Groups[1].Value;

            var viaRecord = items.Where(m => m.Record == record).FirstOrDefault();
            var stat = viaRecord?.Stats.Where(m => m.Stat == "MasteryEnumeration").FirstOrDefault()?.TextValue;
            if (stat != null && stat.Length >= 2) {
                return stat.Substring(stat.Length - 2);
            }

            return string.Empty;
        }

        /// <summary>
        ///     +1 to Occultist
        ///     Or similar
        /// </summary>
        /// <param name="result"></param>
        /// <param name="item"></param>
        private static void GetSpecialMasteryStats(List<DatabaseItemStat> result, DatabaseItem item,
            IEnumerable<DatabaseItem> items) {
            var stats = item.Stats;

            // Special case for "+1 to occultist" etc, since its a combination of 2 stats
            for (int i = 1; i <= 4; i++) {
                bool hasLevel = stats.Any(m => m.Stat.Equals("augmentMasteryLevel" + i));
                bool hasClass = stats.Any(m => m.Stat.Equals("augmentMasteryName" + i));
                if (hasLevel && hasClass) {
                    float amount = stats.Where(m => m.Stat.Equals("augmentMasteryLevel" + i)).FirstOrDefault().Value;
                    string profession = stats.Where(m => m.Stat.Equals("augmentMasteryName" + i)).FirstOrDefault()
                        .TextValue;


                    var professionTag = ExtractClassFromRecord(profession, items);

                    result.Add(new DatabaseItemStat {
                        Parent = item,
                        Stat = "augmentMastery" + i,
                        Value = amount,
                        TextValue = professionTag
                    });
                }
                else
                    break;
            }
        }


        private static string GetSkillDisplayName(IEnumerable<DatabaseItem> items, string skillRecord) {
            var skill = items?.Where(m => m.Record == skillRecord).FirstOrDefault()?.Stats;
            if (skill != null) {
                // Get the tag-name from the skill
                var skillNameEntry = skill.Where(m => m.Stat.Equals("skillDisplayName")).FirstOrDefault();
                if (skillNameEntry?.TextValue != null)
                    return skillNameEntry.TextValue;

                // It may be without a name, but reference a pet skill
                var petSkillName = skill.Where(m => m.Stat.Equals("petSkillName"));
                if (petSkillName.Any())
                    return GetSkillDisplayName(items, petSkillName.FirstOrDefault().TextValue);

                // The pet skill may in turn reference a buff skill, or the original skill might.
                var buffSkillName = skill.Where(m => m.Stat.Equals("buffSkillName"));
                if (buffSkillName.Any())
                    return GetSkillDisplayName(items, buffSkillName.FirstOrDefault().TextValue);
            }
            else {
                Logger.Warn($"Could not find skill \"{skillRecord}\", broken mod?");
            }
            return null;
        }


        public static string GetRootSkillRecord(List<DatabaseItem> statLoader, string skillRecord) {
            //var skill = allRecords.Where(m => m.Record.Equals(skillRecord))

                var item = statLoader.FirstOrDefault(m => m.Record == skillRecord);
                var skill = item?.Stats;

                // Get the tag-name from the skill
                var skillNameEntry = skill.FirstOrDefault(m => m.Stat.Equals("skillDisplayName"));
                if (skillNameEntry?.TextValue != null)
                    return item.Record;

                // It may be without a name, but reference a pet skill
                var petSkillName = skill.Where(m => m.Stat.Equals("petSkillName"));
                if (petSkillName.Any())
                    return GetRootSkillRecord(statLoader, petSkillName.FirstOrDefault().TextValue);

                // The pet skill may in turn reference a buff skill, or the original skill might.
                var buffSkillName = skill.Where(m => m.Stat.Equals("buffSkillName"));
                if (skill.Any(m => m.Stat.Equals("buffSkillName")))
                    return GetRootSkillRecord(statLoader, buffSkillName.FirstOrDefault().TextValue);

            return null;
        }

        /// <summary>
        ///     +1 to Owl Bash
        ///     Or similar
        /// </summary>
        private static void GetSpecialSkillAugments(
            List<DatabaseItemStat> result,
            DatabaseItem item,
            IEnumerable<DatabaseItem> items,
            List<DatabaseItem> skills,
            Dictionary<string, string> tags
        ) {
            var stats = item.Stats;
            // Special case for "+1 to specific skill" etc, since its a combination of 2 stats
            for (int i = 1; i <= 4; i++) {
                bool hasLevel = stats.Any(m => m.Stat.Equals("augmentSkillLevel" + i));
                bool hasClass = stats.Any(m => m.Stat.Equals("augmentSkillName" + i));
                if (hasLevel && hasClass) {
                    float amount = stats.FirstOrDefault(m => m.Stat.Equals("augmentSkillLevel" + i)).Value;
                    string skillRecord = stats.FirstOrDefault(m => m.Stat.Equals("augmentSkillName" + i))
                        .TextValue;

                    // Get the tag-name from the skill
                    string skillNameEntry = GetSkillDisplayName(skills, skillRecord);
                    if (skillNameEntry == null)
                        continue;

                    // Get the real name from the tag
                    var tag = tags.ContainsKey(skillNameEntry) ? tags[skillNameEntry] : null;

                    if (tag == null)
                        continue;

                    // Store the skill name and +amount
                    result.Add(new DatabaseItemStat {
                        Parent = item,
                        Stat = "augmentSkill" + i,
                        Value = amount,
                        TextValue = tag
                    });


                    // Extra data (class and tier for +1 to someskill)
                    string rootSkill = GetRootSkillRecord(skills, skillRecord);


                    var dbstat = (skills.FirstOrDefault(m => m.Record == rootSkill)?.Stats).FirstOrDefault(m => m.Stat == "skillTier");
                    if (dbstat != null) {
                        string skillClass = ExtractClassFromRecord(skillRecord, items);
                        result.Add(new DatabaseItemStat {
                            Parent = item,
                            Stat = "augmentSkill" + i + "Extras",
                            Value = dbstat.Value,
                            TextValue = skillClass
                        });
                    }
                }
                else
                    break;
            }
        }

        private IEnumerable<DatabaseItemStat> CreateSpecialRecords(
            IEnumerable<DatabaseItem> items,
            Dictionary<string, string> tags
            ) {
            List<DatabaseItemStat> result = new List<DatabaseItemStat>();
            foreach (var item in items) {
                GetSpecialMasteryStats(result, item, items);
                GetSpecialSkillAugments(result, item, items, _skills, tags);
            }

            return result;
        }


        private static void MapItemNames(List<DatabaseItem> items, IDictionary<string, string> tags) {
            for (int i = 0; i < items.Count; i++) {
                var item = items[i];
                if (!item.Slot.StartsWith("Loot")) {
                    var keytags = new[] {
                        item.GetTag("itemStyleTag"), item.GetTag("itemNameTag", "description"),
                        item.GetTag("itemQualityTag")
                    };
                    List<string> finalTags = new List<string>();
                    foreach (var tag in keytags) {
                        if (tags.ContainsKey(tag)) {
                            finalTags.Add(tags[tag]);
                        }
                    }

                    items[i].Name = string.Join(" ", finalTags).Trim();
                }
            }
        }

        public void Dispose() {
            _skills.Clear();
        }
    }
}
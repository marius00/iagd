using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;
using EvilsoftCommons;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Arc;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Utilities;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Parsers.GameDataParsing.Service {
    class ArzParsingWrapper {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ArzParsingWrapper));
        private readonly ItemTagAccumulator _tagAccumulator = new ItemTagAccumulator();
        public List<DatabaseItem> Items { get; private set; }

        public List<ItemTag> Tags => _tagAccumulator.Tags;

        public void LoadItems(
            List<string> arzFiles,
            ProgressTracker tracker
        ) {
            tracker.MaxValue = arzFiles.Select(File.Exists).Count();

            // Developers can flip this switch to get a full dump of the GD database. 
            // Setting it to true will cause the parsing to skip a lot of data that IA does not need.
            const bool skipIrrelevantStats = true;  // "skipLots"

            ItemAccumulator accumulator = new ItemAccumulator();
            try {
                foreach (string arzFile in arzFiles) {
                    if (File.Exists(arzFile)) {
                        Logger.Debug($"Parsing / Loading items from {arzFile}");
                        Parser.Arz.ArzParser.LoadItemRecords(arzFile, skipIrrelevantStats).ForEach(accumulator.Add);
                        tracker.Increment();
                    }
                    else {
                        Logger.Debug($"Ignoring non existing arz file {arzFile}");
                    }
                }
            }
            catch (ArgumentException ex) {
                Logger.Warn(ex.Message, ex);
                MessageBox.Show("Game installation is corrupted.\nPlease verify the integrity of your Grim Dawn installation and try again.\n\n(Easily done in steam)");
                throw ex;
            }

            Items = accumulator.Items;
        }

        private void LoadTags(string file) {
            bool isTagfileLocked = IOHelper.IsFileLocked(new FileInfo(file));
            TemporaryCopy tempfile = isTagfileLocked ? new TemporaryCopy(file) : null;
            Logger.Debug($"Loading tags from {file}");

            List<IItemTag> tags = Parser.Arz.ArzParser.ParseArcFile(isTagfileLocked ? tempfile.Filename : file);
            tags.ForEach(m => _tagAccumulator.Add(m.Tag, m.Name));
            Logger.Debug($"Loaded {tags.Count} tags from {file}");
        }

        public void LoadTags(
            List<string> tagfiles,
            string localizationFile,
            ProgressTracker tracker
        ) {
            int numFiles = tagfiles.Count - tagfiles.Where(string.IsNullOrEmpty).Count() + (string.IsNullOrEmpty(localizationFile) ? 0 : 1);
            tracker.MaxValue = numFiles;

            // Load tags in a prioritized order
            foreach (var tagfile in tagfiles) {
                if (File.Exists(tagfile)) {
                    Logger.Debug($"Loading tags from {tagfile}");
                    LoadTags(tagfile);
                }
                else {
                    Logger.Debug($"Ignoring non-existing tagfile {tagfile}");
                }

                tracker.Increment();
            }

            // User override for some tags
            var localizationLoader = new LocalizationLoader();
            if (!string.IsNullOrEmpty(localizationFile) && localizationLoader.Load(localizationFile)) {
                Logger.Debug($"Loading tags from {localizationFile}");

                var tags = localizationLoader.GetItemTags();
                foreach (var tag in tags) {
                    _tagAccumulator.Add(tag.Tag, tag.Name);
                }

                Logger.Debug($"Loaded {tags.Count} tags from {localizationFile}");
                tracker.Increment();
            }

            tracker.MaxProgress();
        }


        public void MapItemNames(ProgressTracker tracker) {
            var tags = Tags;

            tracker.MaxValue = Items.Count;

            Parallel.For(0, Items.Count, i => {
                var item = Items[i];
                if (!item.Slot.StartsWith("Loot")) {
                    var keytags = new[] {
                        item.GetTag("itemStyleTag"), item.GetTag("itemNameTag", "description"),
                        item.GetTag("itemQualityTag")
                    };

                    List<string> finalTags = new List<string>();
                    foreach (var tag in keytags) {
                        var t = tags.FirstOrDefault(m => m.Tag == tag);
                        if (t != null) {
                            finalTags.Add(t.Name);
                        }
                    }

                    Items[i].Name = string.Join(" ", finalTags).Trim();
                }

                tracker.Increment();

            });

            tracker.MaxProgress();
        }


        public void RenamePetStats(ProgressTracker tracker) {
            Logger.Debug("Detecting records with pet bonus stats..");

            var petRecords = Items.SelectMany(m => m.Stats.Where(s => s.Stat == "petBonusName")
                    .Select(s => s.TextValue))
                .ToList(); // ToList for performance reasons

            var petItems = Items.Where(m => petRecords.Contains(m.Record)).ToList();
            tracker.MaxValue = petItems.Count;
            foreach (var petItem in petItems) {
                var stats = petItem.Stats.Select(s => new DatabaseItemStat {
                    Stat = "pet" + s.Stat,
                    TextValue = s.TextValue,
                    Value = s.Value,
                    Parent = s.Parent
                }).ToList();

                petItem.Stats.Clear();
                petItem.Stats = stats;
                tracker.Increment();
            }

            Items.RemoveAll(m => petRecords.Contains(m.Record));
            Items.AddRange(petItems);

            tracker.MaxProgress();
            Logger.Debug($"Classified {petItems.Count()} records as pet stats");
        }

        /// <summary>
        /// Primarily parses skills and maps item-skill
        /// </summary>
        /// <param name="itemSkillDao"></param>
        /// <param name="tracker"></param>
        public void ParseComplexItems(IItemSkillDao itemSkillDao, ProgressTracker tracker) {
            var mappedTags = _tagAccumulator.MappedTags;
            var skillParser = new ComplexItemParser(Items, mappedTags);
            skillParser.Generate(tracker);
            itemSkillDao.Save(skillParser.Skills, false);
            itemSkillDao.Save(skillParser.SkillItemMapping, false);
        }


        public List<DatabaseItemStat> GenerateSpecialRecords(ProgressTracker tracker) {
            var skills = Items
                .Where(m => m.Record.Contains("/skills/"))
                .ToList();
            List<DatabaseItemStat> result = new List<DatabaseItemStat>();

            var filtered = Items.Where(m => m.Id != 0).ToList();
            tracker.MaxValue = filtered.Count;
            foreach (var item in filtered) {
                ArzParser.GetSpecialMasteryStats(result, item, Items);
                ArzParser.GetSpecialSkillAugments(result, item, Items, skills, _tagAccumulator.MappedTags);
                tracker.Increment();
            }

            return result;
        }
    }
}

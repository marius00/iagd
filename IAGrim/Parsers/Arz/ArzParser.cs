using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataAccess;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Arc;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Parsers.Arz {
    public class ArzParser {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ArzParser));
        private readonly IDatabaseSettingDao _databaseSettingDao;
        


        public ArzParser(IDatabaseSettingDao databaseSettingDao) {
            _databaseSettingDao = databaseSettingDao;
        }
        
        public static void LoadIconsOnly(string grimdawnLocation) {
            Logger.Debug("Icon loading requested");
            {
                var arcItemsFile = GrimFolderUtility.FindArcFile(grimdawnLocation, "items.arc");
                if (!string.IsNullOrEmpty(arcItemsFile)) {
                    Logger.Debug($"Loading vanilla icons from {arcItemsFile}");
                    LoadIcons(arcItemsFile);
                }
                else {
                    Logger.Warn("Could not find the vanilla icons, skipping.");
                }
            }
            
            foreach (string path in GrimFolderUtility.GetGrimExpansionFolders(grimdawnLocation)) {
                var arcItemsFile = GrimFolderUtility.FindArcFile(path, "items.arc");
                if (!string.IsNullOrEmpty(arcItemsFile)) {
                    Logger.Debug($"Loading expansion icons from {arcItemsFile}");
                    LoadIcons(arcItemsFile);
                }
                else {
                    Logger.Warn("Could not find the expansion, skipping.");
                }

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

            try {
                DDSImageReader.ExtractItemIcons(arcItemfile, GlobalPaths.StorageFolder);
            }
            catch (ArgumentException ex) {
                // Ideally we'd like to catch the specific exception, but the available log files don't contain the exception name..
                Logger.Error(ex.Message, ex);
                MessageBox.Show("Unable to parse icons, ARZ file is corrupted.\nIf you are using steam, please verify the install integrity.", "Corrupted GD installation", MessageBoxButtons.OK);
            }
        }

        public static string ExtractClassFromRecord(string record, IEnumerable<DatabaseItem> items) {
            Regex playerClassRx = new Regex(@".*/player(class\d+)/.*");
            // "MasteryEnumeration"	"SkillClass24" => tagSkillClassName24
            var rExSkillClass = playerClassRx.Match(record);
            if (rExSkillClass.Success && rExSkillClass.Groups.Count == 2)
                return rExSkillClass.Groups[1].Value;

            var viaRecord = items.FirstOrDefault(m => m.Record == record);
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
        public static void GetSpecialMasteryStats(List<DatabaseItemStat> result, DatabaseItem item,
            IEnumerable<DatabaseItem> items) {
            var stats = item.Stats;

            // Special case for "+1 to occultist" etc, since its a combination of 2 stats
            for (int i = 1; i <= 4; i++) {
                bool hasLevel = stats.Any(m => m.Stat.Equals("augmentMasteryLevel" + i));
                bool hasClass = stats.Any(m => m.Stat.Equals("augmentMasteryName" + i));
                if (hasLevel && hasClass) {
                    float amount = stats.First(m => m.Stat.Equals("augmentMasteryLevel" + i)).Value;
                    string profession = stats.First(m => m.Stat.Equals("augmentMasteryName" + i)).TextValue;


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
                var skillNameEntry = skill.FirstOrDefault(m => m.Stat.Equals("skillDisplayName"));
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
        public static void GetSpecialSkillAugments(
            List<DatabaseItemStat> result,
            DatabaseItem item,
            List<DatabaseItem> items,
            List<DatabaseItem> skills,
            Dictionary<string, string> tags
        ) {
            var stats = item.Stats;
            // Special case for "+1 to specific skill" etc, since its a combination of 2 stats
            for (int i = 1; i <= 4; i++) {
                bool hasLevel = stats.Any(m => m.Stat.Equals("augmentSkillLevel" + i));
                bool hasClass = stats.Any(m => m.Stat.Equals("augmentSkillName" + i));
                if (hasLevel && hasClass) {
                    float amount = stats.First(m => m.Stat.Equals("augmentSkillLevel" + i)).Value;
                    string skillRecord = stats.First(m => m.Stat.Equals("augmentSkillName" + i)).TextValue;

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
    }
}
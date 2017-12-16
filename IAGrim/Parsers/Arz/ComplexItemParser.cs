using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using IAGrim.Parsers.Arz.dto;
using log4net;

namespace IAGrim.Parsers.Arz {
    internal class ComplexItemParser {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ComplexItemParser));
        private readonly List<IItem> _items;
        private readonly Dictionary<string, string> _tags;

        public ComplexItemParser(List<IItem> items, Dictionary<string, string> tags) {
            _items = items;
            _tags = tags;
        }

        public Dictionary<string, List<string>> SkillItemMapping { get; } = new Dictionary<string, List<string>>();
        public ISet<ItemGrantedSkill> Skills { get; } = new HashSet<ItemGrantedSkill>();

        public void Generate() {
            Logger.Debug($"Generating skills for {_items.Count} items.");
            foreach (var item in _items) {
                var skill = GetSkill(item);
                if (skill != null) {
                    if (!SkillItemMapping.ContainsKey(skill.Record))
                        SkillItemMapping[skill.Record] = new List<string>();

                    SkillItemMapping[skill.Record].Add(item.Record);
                    Skills.Add(skill);
                }
            }


            var numItemsWithSkills = SkillItemMapping.Values.Sum(m => m.Count);
            Logger.Debug($"Generated {Skills.Count} skills for {numItemsWithSkills} items.");
        }

        private ItemGrantedSkill GetSkill(IItem item) {
            try {
                if (item.Stats.Any(m => m.Stat.Equals("itemSkillName"))) {
                    var record = item.Stats.FirstOrDefault(m => m.Stat.Equals("itemSkillName"))?.TextValue;

                    var stats = _items.FirstOrDefault(m => m.Record.Equals(record))?.Stats;
                    // Doesn't exist??
                    if (stats == null)
                        return null;

                    // Some items (like the Apothecary's touch) just references a subskill
                    var subSkill = stats.FirstOrDefault(m => m.Stat == "buffSkillName");
                    if (subSkill != null) {
                        var sub = _items.FirstOrDefault(m => m.Record == subSkill.TextValue);
                        if (sub != null) {
                            stats = sub.Stats;
                            record = sub.Record;
                        }
                    }


                    var nameTag = stats.FirstOrDefault(m => m.Stat.Equals("skillDisplayName"))?.TextValue ?? string.Empty;
                    var descTag = stats.FirstOrDefault(m => m.Stat.Equals("skillBaseDescription"))?.TextValue ?? string.Empty;


                    var name = _tags.ContainsKey(nameTag) ? _tags[nameTag] : null;
                    var desc = _tags.ContainsKey(descTag) ? _tags[descTag] : null;

                    var level = item.Stats.FirstOrDefault(m => m.Stat.Equals("itemSkillLevelEq"))?.TextValue;
                    long l = 0;
                    if (!string.IsNullOrEmpty(level))
                        long.TryParse(level, out l);


                    return new ItemGrantedSkill {
                        Record = record,
                        Name = name,
                        Description = desc,
                        Level = l,
                        Trigger = item.Stats.FirstOrDefault(m => m.Stat.Equals("itemSkillAutoController"))?.TextValue
                    };
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            return null;
        }
    }
}
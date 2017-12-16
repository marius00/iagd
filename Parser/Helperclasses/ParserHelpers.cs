using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Helperclasses {
    public class ParserHelpersR<T> where T : IItemStatR {


        public static string GetRootSkillRecord(Func<string, IEnumerable<T>> statLoader, string skillRecord) {
            //var skill = allRecords.Where(m => m.Record.Equals(skillRecord));
            var skill = statLoader(skillRecord);


            // Get the tag-name from the skill
            var skillNameEntry = skill.Where(m => m.Stat.Equals("skillDisplayName")).FirstOrDefault();
            if (skillNameEntry?.TextValue != null)
                return skillNameEntry.Record;

            // It may be without a name, but reference a pet skill
            var petSkillName = skill.Where(m => m.Stat.Equals("petSkillName"));
            if (petSkillName.Any())
                return GetRootSkillRecord(statLoader, petSkillName.FirstOrDefault().TextValue);

            // The pet skill may in turn reference a buff skill, or the original skill might.
            var buffSkillName = skill.Where(m => m.Stat.Equals("buffSkillName"));
            if (buffSkillName.Any())
                return GetRootSkillRecord(statLoader, buffSkillName.FirstOrDefault().TextValue);

            return null;
        }


        public static string GetBitmap(IEnumerable<T> Tags) {
            string result = string.Empty;
            if (Tags != null) {
                if (Tags.Any(m => "bitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "bitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => "relicBitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "relicBitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => "shardBitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "shardBitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => m.Stat.Contains("itmap")))
                    result = Tags.Where(m => m.Stat.Contains("itmap")).FirstOrDefault().TextValue;
            }

            // No icon? Not a problem!
            if (string.IsNullOrEmpty(result))
                return string.Empty;

            return string.Format("{0}.png", System.IO.Path.GetFileName(result.Replace(".dbr", ".tex")));

        }
    }
    public class ParserHelpers<T> where T : IItemStatI {


        public static long GetRootSkillRecord(Func<string, IEnumerable<T>> statLoader, string skillRecord) {
            //var skill = allRecords.Where(m => m.Record.Equals(skillRecord));
            var skill = statLoader(skillRecord);


            // Get the tag-name from the skill
            var skillNameEntry = skill.Where(m => m.Stat.Equals("skillDisplayName")).FirstOrDefault();
            if (skillNameEntry?.TextValue != null)
                return skillNameEntry.Id;

            // It may be without a name, but reference a pet skill
            var petSkillName = skill.Where(m => m.Stat.Equals("petSkillName"));
            if (petSkillName.Any())
                return GetRootSkillRecord(statLoader, petSkillName.FirstOrDefault().TextValue);

            // The pet skill may in turn reference a buff skill, or the original skill might.
            var buffSkillName = skill.Where(m => m.Stat.Equals("buffSkillName"));
            if (buffSkillName.Any())
                return GetRootSkillRecord(statLoader, buffSkillName.FirstOrDefault().TextValue);

            return -1;
        }


        public static string GetBitmap(IEnumerable<T> Tags) {
            string result = string.Empty;
            if (Tags != null) {
                if (Tags.Any(m => "bitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "bitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => "relicBitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "relicBitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => "shardBitmap".Equals(m.Stat)))
                    result = Tags.Where(m => "shardBitmap".Equals(m.Stat)).FirstOrDefault().TextValue;

                else if (Tags.Any(m => m.Stat.Contains("itmap")))
                    result = Tags.Where(m => m.Stat.Contains("itmap")).FirstOrDefault().TextValue;
            }

            // No icon? Not a problem!
            if (string.IsNullOrEmpty(result))
                return string.Empty;

            return string.Format("{0}.png", System.IO.Path.GetFileName(result.Replace(".dbr", ".tex")));

        }
    }
}

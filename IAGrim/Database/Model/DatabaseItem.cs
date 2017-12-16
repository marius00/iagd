using IAGrim.Parsers.Arz;
using IAGrim.Services.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    public class DatabaseItem : IComparable, ICloneable {
        public virtual long Id { get; set; }


        public virtual string Record { get; set; }
        public virtual IList<DatabaseItemStat> Stats { get; set; }

        public virtual string Name { set; get; }

        public virtual string GetTag(string tag) {
            if (Stats.Any(m => tag.Equals(m.Stat)))
                return Stats.FirstOrDefault(m => tag.Equals(m.Stat))?.TextValue;
            return
                string.Empty;
        }
        public virtual string GetTag(string tag, string fallback) {
            if (Stats.Any(m => tag.Equals(m.Stat)))
                return Stats.FirstOrDefault(m => tag.Equals(m.Stat))?.TextValue;
            return
                GetTag(fallback);
        }

        public static float GetMinimumLevel(IEnumerable<DBSTatRow> Stats) {
            if (Stats.Any(m => "levelRequirement".Equals(m.Stat)))
                return (float)Stats.Where(m => "levelRequirement".Equals(m.Stat)).FirstOrDefault().Value;
            else
                return 0;
        }

        /// <summary>
        /// Get the reference to this items bitmap
        /// </summary>
        public static string GetBitmap(ISet<DBSTatRow> stats) {
            string result = string.Empty;
            if (stats != null) {
                if (stats.Any(m => "bitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "bitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => "relicBitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "relicBitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => "shardBitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "shardBitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => m.Stat.Contains("itmap")))
                    result = stats.FirstOrDefault(m => m.Stat.Contains("itmap"))?.TextValue;
                /*
                    
                    else if (Stats.Any(m => "artifactFormulaBitmapName".Equals(m.Stat)))
                        return Stats.Where(m => "artifactFormulaBitmapName".Equals(m.Stat)).FirstOrDefault().TextValue;*/
            }

            return $"{Path.GetFileName(result?.Replace(".dbr", ".tex"))}.png";
        }

        public static string GetSlot(IEnumerable<DBSTatRow> Stats) {
            if (Stats.Any(m => "Class".Equals(m.Stat)))
                return Stats.Where(m => "Class".Equals(m.Stat)).FirstOrDefault().TextValue;
            else
                return string.Empty;
        }


        public virtual string Slot {
            get {
                if (Stats.Any(m => "Class".Equals(m.Stat)))
                    return Stats.Where(m => "Class".Equals(m.Stat)).FirstOrDefault().TextValue;
                else
                    return string.Empty;
            }
        }

        public static string GetRarity(IEnumerable<DBSTatRow> Stats) {
            if (Stats.Any(m => "artifactClassification".Equals(m.Stat))) {
                return "Blue";
            }
            else if (Stats.Any(m => "itemClassification".Equals(m.Stat))) {
                string value = Stats.Where(m => "itemClassification".Equals(m.Stat)).FirstOrDefault().TextValue;

                if ("Epic".Equals(value))
                    return "Blue";

                else if ("Legendary".Equals(value))
                    return "Epic";

                else if ("Rare".Equals(value))
                    return "Green";

                else if ("Magical".Equals(value))
                    return "Yellow";
            }

            return "Unknown";

        }

        public override int GetHashCode() {
            return Record.GetHashCode();
        }

        public override bool Equals(object obj) {
            DatabaseItem other = obj as DatabaseItem;
            if (other != null)
                return Record.Equals(other.Record);

            return base.Equals(obj);
        }

        public virtual int CompareTo(object obj) {
            DatabaseItem other = obj as DatabaseItem;
            if (other != null) {
                return Record.CompareTo(other.Record);
            }

            return 0;
        }


        public virtual object Clone() {
            return this.MemberwiseClone();
        }
    }
}

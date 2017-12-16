using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDCharSkill {
        private const String PATTERN_PLAYERCLASS = "records/skills/playerclass(.*)/.*";
        private const String PATTERN_MASTERY = "records/skills/playerclass(.*)/_classtraining_class(.*).dbr";
        private const String PATTERN_DEVOTION = "records/skills/devotion/(.*).dbr";
        private const String PATTERN_MASTER_SKILL = "records/skills/playerclass(.*)/[^_]+(.*).dbr";

        public String Name { get; private set; }
        public String AutoCastSkill { get; private set; }
        public String AutoCastController { get; private set; }
        public int Level { get; private set; }
        public int DevotionLevel { get; private set; }
        public int Experience { get; private set; }
        public int Active { get; private set; }
        public bool Enabled { get; private set; }
        public int PlayerClass {
            get {
                
                foreach (Match match in new Regex(PATTERN_PLAYERCLASS).Matches(Name)) {
                    int n;
                    if (int.TryParse(match.Groups[1].Value, out n))
                        return n;
                }

                return -1;
            }
        }
        public bool IsMastery {
            get {
                return new Regex(PATTERN_MASTERY).IsMatch(Name);
            }
        }

        public bool IsMasterySkill {
            get {
                return new Regex(PATTERN_MASTER_SKILL).IsMatch(Name);
            }
        }            
        
        public bool IsDevotion {
            get {
                return new Regex(PATTERN_DEVOTION).IsMatch(Name);
            }
        }

        public void Read(GDCryptoDataBuffer reader) {
            Name = reader.ReadCryptoStringUnchecked();
            Level = reader.ReadCryptoIntUnchecked();
            Enabled = reader.ReadCryptoByteUnchecked() == 1;
            DevotionLevel = reader.ReadCryptoIntUnchecked();
            Experience = reader.ReadCryptoIntUnchecked();
            Active = reader.ReadCryptoIntUnchecked();
            reader.ReadCryptoByteUnchecked();
            reader.ReadCryptoByteUnchecked();
            AutoCastSkill = reader.ReadCryptoStringUnchecked();
            AutoCastController = reader.ReadCryptoStringUnchecked();
        }
    }
}

using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDCharBio {
        private const int VERSION = 8;
        private const int BLOCK = 2;


        public int Level { get; set; }
        public int Experience { get; set; }
        public int ModifierPoints { get; set; }
        public int SkillPoints { get; set; }
        public int DevotionPoints { get; set; }
        public int TotalDevotion { get; set; }
        public float TotalStrength { get; set; }
        public float TotalAgility { get; set; }
        public float TotalIntelligence { get; set; }
        public float Health { get; set; }
        public float Energy { get; set; }

        /// <summary>
        /// Attribute points spent
        /// </summary>
        public int Strength {
            get {
                return (int)((TotalStrength - 50) / 8);
            }
        }

        /// <summary>
        /// Attribute points spent
        /// </summary>
        public int Agility {
            get {
                return (int)((TotalAgility - 50) / 8);
            }
        }

        /// <summary>
        /// Attribute points spent
        /// </summary>
        public int Intelligence {
            get {
                return (int)((TotalIntelligence - 50) / 8);
            }
        }

        public void Read(GDCryptoDataBuffer reader) {
            reader.ReadBlockStart(BLOCK);


            int version = reader.ReadCryptoIntUnchecked();
            if (version != VERSION)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            Level = reader.ReadCryptoIntUnchecked();
            Experience = reader.ReadCryptoIntUnchecked();
            ModifierPoints = reader.ReadCryptoIntUnchecked();
            SkillPoints = reader.ReadCryptoIntUnchecked();
            DevotionPoints = reader.ReadCryptoIntUnchecked();
            TotalDevotion = reader.ReadCryptoIntUnchecked();

            TotalStrength = reader.ReadCryptoFloatUnchecked();
            TotalAgility = reader.ReadCryptoFloatUnchecked();
            TotalIntelligence = reader.ReadCryptoFloatUnchecked();
            Health = reader.ReadCryptoFloatUnchecked();
            Energy = reader.ReadCryptoFloatUnchecked();

            // end marker
            reader.ReadBlockEnd();
        }
    }
}
using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDCharSkillList {
        private const int VERSION = 5;
        private const int BLOCK = 8;
        public List<GDCharSkill> Skills { get; private set; }

        public void Read(GDCryptoDataBuffer reader) {
            reader.ReadBlockStart(BLOCK);

            int version = reader.ReadCryptoIntUnchecked();
            if (version != VERSION)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            Skills = new List<GDCharSkill>();
            int numSkills = reader.ReadCryptoIntUnchecked();
            for (int i = 0; i < numSkills; i++) {
                var skill = new GDCharSkill();
                skill.Read(reader);
                Skills.Add(skill);
            }

            /*
            int numItemSkills = reader.ReadCryptoIntUnchecked();
            for (int i = 0; i < numItemSkills; i++) {
            }

            reader.ReadBlockEnd();
            */
        }
    }
}

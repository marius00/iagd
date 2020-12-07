using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using StatTranslator;

namespace IAGrim.Database.Model {
    public class SkillModifierStat {
        public string Name { get; set; }
        public string Class { get; set; }
        public float? Tier { get; set; }
        public bool IsMonsterInfrequent { get; set; }

        public virtual IList<TranslatedStat> Translated {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return RuntimeSettings.StatManager.ProcessSkillModifierStats(new HashSet<IItemStat>(Tags), Name, Class, Tier);
            }
        }

        public ISet<DBStatRow> Tags { get; set; }
    }
}

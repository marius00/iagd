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
    public class PlayerItemSkill {
        public string PlayerItemRecord { get; set; }
        public string Description { get; set; }

        public long? Level { get; set; }

        public string Record { get; set; }

        public string Name { get; set; }

        public long StatsId { get; set; }

        public string TriggerRecord { get; set; }

        public TranslatedStat Trigger => TriggerRecord == null ? null : GlobalSettings.StatManager.TranslateSkillAutoController(TriggerRecord);

        public ISet<DBSTatRow> Tags {
            get;
            set;
        }

        public IList<TranslatedStat> PetStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.PET);
            }
        }

        public IList<TranslatedStat> HeaderStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.HEADER);
            }
        }

        public IList<TranslatedStat> BodyStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.BODY);
            }
        }
    }
}

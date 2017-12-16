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
    public class BaseItem {
        public virtual string BaseRecord { get; set; }
        public virtual string PrefixRecord { get; set; }
        public virtual string SuffixRecord { get; set; }
        public virtual string ModifierRecord { get; set; }
        public virtual string TransmuteRecord { get; set; }
        public virtual string PetRecord { get; set; }
        public virtual string MateriaRecord { get; set; }
        public virtual string EnchantmentRecord { get; set; }

        public virtual ISet<DBSTatRow> Tags {
            get;
            set;
        }


        public virtual IList<TranslatedStat> PetStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.PET);
            }
        }

        public virtual IList<TranslatedStat> HeaderStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.HEADER);
            }
        }

        public virtual IList<TranslatedStat> BodyStats {
            get {
                if (Tags == null)
                    return new List<TranslatedStat>();
                return GlobalSettings.StatManager.ProcessStats(new HashSet<IItemStat>(Tags), TranslatedStatType.BODY);
            }
        }
    }
}

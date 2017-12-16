using StatTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Controller {

    public class JsonItem {
        public string BaseRecord { get; set; }
        public string Icon { get; set; }
        public string Quality { get; set; }

        public string Name { get; set; }
        public string Socket { get; set; }
        public float Level { get; set; }
        public object[] URL { get; set; }
        public uint NumItems { get; set; }

        public int Type { get; set; }
        public string[] Buddies { get; set; }
        public bool HasRecipe { get; set; }

        public IList<JsonStat> HeaderStats { get; set; }
        public IList<JsonStat> BodyStats { get; set; }
        public IList<JsonStat> PetStats { get; set; }

        public int GreenRarity { get; set; }

        public JsonSkill Skill { get; set; }
    }
}
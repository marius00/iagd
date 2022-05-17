using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Utilities;
using StatTranslator;

namespace IAGrim.Database.Model {
    public class CollectionItemAggregateRow {
        public long Num { get; set; }
        public string Quality { get; set; }
        public string Slot { get; set; }
        public string TranslatedSlot => SlotTranslator.Translate(RuntimeSettings.Language, Slot ?? "");

    }
}

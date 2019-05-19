﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Dto {

    public class ItemSearchRequest {
        public string Wildcard { get; set; }
        public List<string[]> Filters { get; set; }
        public float MinimumLevel { get; set; }
        public float MaximumLevel { get; set; }
        public string Rarity { get; set; }
        public string[] Slot { get; set; }
        public bool PetBonuses { get; set; }
        public bool IsRetaliation { get; set; }
        public string Mod { get; set; }
        public bool IsHardcore { get; set; }
        public int PrefixRarity { get; set; }

        public List<string> Classes { get; set; }

        public bool SocketedOnly { get; set; }

        public bool RecentOnly { get; set; }

        public bool IsEmpty {
            get {
                if (!String.IsNullOrEmpty(Wildcard))
                    return false;
                if (Filters.Count > 0)
                    return false;
                if (MinimumLevel >= 1 || MaximumLevel <= 84)
                    return false;
                if (!String.IsNullOrEmpty(Rarity) || Slot != null)
                    return false;
                if (PetBonuses || IsRetaliation || Classes.Count > 0 || SocketedOnly || RecentOnly)
                    return false;
                return true;
            }
        }
    }
}

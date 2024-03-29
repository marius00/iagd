﻿using System;
using System.Collections.Generic;
using IAGrim.Services.ItemReplica;

namespace IAGrim.UI.Controller.dto {

    public class JsonItem : IComparable<JsonItem> {
        public string UniqueIdentifier { get; set; }
        /// <summary>
        /// Used to identify "identical" items which can be merged in the UI
        /// </summary>
        public string MergeIdentifier { get; set; }
        public string BaseRecord { get; set; }
        public string Icon { get; set; }
        public string Quality { get; set; }

        public string Name { get; set; }
        public string Socket { get; set; }
        public string Slot { get; set; }
        public float Level { get; set; }
        public object[] URL { get; set; }
        public ItemTypeDto Type { get; set; }
        public bool HasRecipe { get; set; }

        public IList<JsonStat> HeaderStats { get; set; }
        public IList<JsonStat> BodyStats { get; set; }
        public IList<JsonStat> PetStats { get; set; }

        public bool HasCloudBackup { get; set; }
        public int GreenRarity { get; set; }
        public JsonSkill Skill { get; set; }
        public bool IsMonsterInfrequent { get; set; }
        public string Extras { get; set; } // TODO: This should be a custom object

        public bool IsHardcore { get; set; }

        public IList<ItemStatInfo> ReplicaStats { get; set; }

        public int CompareTo(JsonItem other) {
            if (other.Type != this.Type) {
                return other.Type - this.Type;
            }

            return this.UniqueIdentifier.CompareTo(other.UniqueIdentifier);
        }
    }
}
using System;
using System.Collections.Generic;
using IAGrim.Services.ItemStats;

namespace IAGrim.UI
{
    internal class FilterEventArgs : EventArgs
    {
        public List<string[]>? Filters { get; set; }

        /// <summary>Per-checkbox numeric stat filters (e.g. Fire &gt;= 30) from the stat filter buttons.</summary>
        public List<StatValueFilter>? NumericFilters { get; set; }

        public bool PetBonuses { get; set; }

        public bool IsRetaliation { get; set; }

        public bool DuplicatesOnly { get; set; }

        public bool SocketedOnly { get; set; }

        public bool RecentOnly { get; set; }

        public bool GrantsSkill { get; set; }


        public bool WithSummonerSkillOnly { get; set; }

        public List<string>? DesiredClass { get; set; }
    }
}
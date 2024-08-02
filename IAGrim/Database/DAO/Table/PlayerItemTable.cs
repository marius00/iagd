﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.DAO.Table {
    static class PlayerItemTable {
        public const string Table = "playeritem";
        public const string Id = "id";
        public const string Record = "baserecord";
        public const string Stackcount = "stackcount";
        public const string Mod = "mod";
        public const string PrefixRarity = "prefixrarity";


        public const string Prefix = "prefixrecord";
        public const string Suffix = "suffixrecord";
        public const string ModifierRecord = "modifierrecord";
        public const string Materia = "materiarecord";
        public const string Seed = "seed";

        public const string CloudId = "cloudid";
        public const string IsCloudSynchronized = "cloud_hassync";
        public const string IsHardcore = "ishardcore";
    }
}

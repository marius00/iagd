using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.DAO.Table {
    static class PlayerItemTable {
        public const string Table = "playeritem";
        public const string Id = "Id";
        public const string Record = "BaseRecord";
        public const string Stackcount = "StackCount";
        public const string Mod = "Mod";
        public const string PrefixRarity = "PrefixRarity";


        public const string Prefix = "PrefixRecord";
        public const string Suffix = "SuffixRecord";
        public const string ModifierRecord = "ModifierRecord";
        public const string Materia = "MateriaRecord";
        public const string Seed = "Seed";

        public const string CloudId = "cloudid";
        public const string IsCloudSynchronized = "cloud_hassync";
        public const string IsHardcore = "IsHardcore";
    }
}

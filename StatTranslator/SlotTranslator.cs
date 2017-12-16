using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {
    public static class SlotTranslator {
        static Dictionary<string, string> SlotMap;
        public static string Translate(string stat) {
            if (SlotMap == null) {
                SlotMap = new Dictionary<string, string>();

                SlotMap["ArmorProtective_Head"] = "Head";
                SlotMap["ArmorProtective_Hands"] = "Hands";
                SlotMap["ArmorProtective_Feet"] = "Feet";
                SlotMap["ArmorProtective_Legs"] = "Legs";
                SlotMap["ArmorProtective_Chest"] = "Chest";
                SlotMap["ArmorProtective_Waist"] = "Belt";
                SlotMap["ArmorJewelry_Medal"] = "Medal";
                SlotMap["ArmorJewelry_Ring"] = "Ring";
                SlotMap["ArmorProtective_Shoulders"] = "Shoulder";
                SlotMap["ArmorJewelry_Amulet"] = "Amulet/Neck";

                SlotMap["WeaponMelee_Dagger"] = "Dagger (1h)";
                SlotMap["WeaponMelee_Mace"] = "Mace (1h)";
                SlotMap["WeaponMelee_Axe"] = "Axe (1h)";
                SlotMap["WeaponMelee_Scepter"] = "Scepter (1h)";
                SlotMap["WeaponMelee_Sword"] = "Sword (1h)";

                SlotMap["WeaponMelee_Sword2h"] = "Sword (2h)";
                SlotMap["WeaponMelee_Mace2h"] = "Mace (2h)";
                SlotMap["WeaponMelee_Axe2h"] = "Axe (2h)";

                SlotMap["WeaponHunting_Ranged1h"] = "Ranged (1h)";
                SlotMap["WeaponHunting_Ranged2h"] = "Ranged (2h)";

                SlotMap["WeaponArmor_Offhand"] = "Offhand";
                SlotMap["WeaponArmor_Shield"] = "Shield";
                SlotMap["ItemRelic"] = "Component";
                SlotMap["ItemArtifact"] = "Relic";
                SlotMap["ItemFactionBooster"] = "Scroll";
            }



            if (!string.IsNullOrEmpty(stat) && SlotMap.ContainsKey(stat))
                return SlotMap[stat];
            else
                return stat;
        }
    }
}

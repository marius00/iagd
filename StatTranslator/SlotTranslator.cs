using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {
    public static class SlotTranslator {
        static Dictionary<string, string> SlotMap;
        public static string Translate(ILocalizedLanguage language, string stat) {
            if (SlotMap == null) {
                SlotMap = new Dictionary<string, string>();
                
                SlotMap["ArmorProtective_Head"] = language.GetTag("iatag_slot_head");
                SlotMap["ArmorProtective_Hands"] = language.GetTag("iatag_slot_hands");
                SlotMap["ArmorProtective_Feet"] = language.GetTag("iatag_slot_feet");
                SlotMap["ArmorProtective_Legs"] = language.GetTag("iatag_slot_legs");
                SlotMap["ArmorProtective_Chest"] = language.GetTag("iatag_slot_chest");
                SlotMap["ArmorProtective_Waist"] = language.GetTag("iatag_slot_belt");
                SlotMap["ArmorJewelry_Medal"] = language.GetTag("iatag_slot_medal");
                SlotMap["ArmorJewelry_Ring"] = language.GetTag("iatag_slot_ring");
                SlotMap["ArmorProtective_Shoulders"] = language.GetTag("iatag_slot_shoulder");
                SlotMap["ArmorJewelry_Amulet"] = language.GetTag("iatag_slot_neck");

                SlotMap["WeaponMelee_Dagger"] = language.GetTag("iatag_slot_dagger1h");
                SlotMap["WeaponMelee_Mace"] = language.GetTag("iatag_slot_mace1h");
                SlotMap["WeaponMelee_Axe"] = language.GetTag("iatag_slot_axe1h");
                SlotMap["WeaponMelee_Scepter"] = language.GetTag("iatag_slot_scepter1h");
                SlotMap["WeaponMelee_Sword"] = language.GetTag("iatag_slot_sword1h");

                SlotMap["WeaponMelee_Sword2h"] = language.GetTag("iatag_slot_sword2h");
                SlotMap["WeaponMelee_Mace2h"] = language.GetTag("iatag_slot_mace2h");
                SlotMap["WeaponMelee_Axe2h"] = language.GetTag("iatag_slot_axe2h");

                SlotMap["WeaponHunting_Ranged1h"] = language.GetTag("iatag_slot_ranged1h");
                SlotMap["WeaponHunting_Ranged2h"] = language.GetTag("iatag_slot_ranged2h");

                SlotMap["WeaponArmor_Offhand"] = language.GetTag("iatag_slot_offhand");
                SlotMap["WeaponArmor_Shield"] = language.GetTag("iatag_slot_shield");
                SlotMap["ItemRelic"] = language.GetTag("iatag_slot_component");
                SlotMap["ItemArtifact"] = language.GetTag("iatag_slot_relic");
                SlotMap["ItemFactionBooster"] = language.GetTag("iatag_slot_scroll");
                SlotMap["ItemEnchantment"] = language.GetTag("iatag_slot_augmentation");
            }



            if (!string.IsNullOrEmpty(stat) && SlotMap.ContainsKey(stat))
                return SlotMap[stat];
            else
                return stat;
        }
    }
}

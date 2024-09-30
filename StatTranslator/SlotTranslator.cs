using System.Collections.Generic;

namespace StatTranslator
{
    public static class SlotTranslator {
        static Dictionary<string, string> SlotMap;
        public static string Translate(ILocalizedLanguage language, string stat) {
            if (SlotMap == null) {
                SlotMap = new Dictionary<string, string>
                {
                    ["ArmorProtective_Head"] = language.GetTag("iatag_slot_head"),
                    ["ArmorProtective_Hands"] = language.GetTag("iatag_slot_hands"),
                    ["ArmorProtective_Feet"] = language.GetTag("iatag_slot_feet"),
                    ["ArmorProtective_Legs"] = language.GetTag("iatag_slot_legs"),
                    ["ArmorProtective_Chest"] = language.GetTag("iatag_slot_chest"),
                    ["ArmorProtective_Waist"] = language.GetTag("iatag_slot_belt"),
                    ["ArmorJewelry_Medal"] = language.GetTag("iatag_slot_medal"),
                    ["ArmorJewelry_Ring"] = language.GetTag("iatag_slot_ring"),
                    ["ArmorProtective_Shoulders"] = language.GetTag("iatag_slot_shoulder"),
                    ["ArmorJewelry_Amulet"] = language.GetTag("iatag_slot_neck"),
                    ["WeaponMelee_Dagger"] = language.GetTag("iatag_slot_dagger1h"),
                    ["WeaponMelee_Mace"] = language.GetTag("iatag_slot_mace1h"),
                    ["WeaponMelee_Axe"] = language.GetTag("iatag_slot_axe1h"),
                    ["WeaponMelee_Scepter"] = language.GetTag("iatag_slot_scepter1h"),
                    ["WeaponMelee_Sword"] = language.GetTag("iatag_slot_sword1h"),
                    ["WeaponMelee_Sword2h"] = language.GetTag("iatag_slot_sword2h"),
                    ["WeaponMelee_Mace2h"] = language.GetTag("iatag_slot_mace2h"),
                    ["WeaponMelee_Axe2h"] = language.GetTag("iatag_slot_axe2h"),
                    ["WeaponMelee_Spear2h"] = language.GetTag("iatag_slot_spear2h"),
                    ["WeaponHunting_Ranged1h"] = language.GetTag("iatag_slot_ranged1h"),
                    ["WeaponHunting_Ranged2h"] = language.GetTag("iatag_slot_ranged2h"),
                    ["WeaponArmor_Offhand"] = language.GetTag("iatag_slot_offhand"),
                    ["WeaponArmor_Shield"] = language.GetTag("iatag_slot_shield"),
                    ["ItemRelic"] = language.GetTag("iatag_slot_component"),
                    ["ItemArtifact"] = language.GetTag("iatag_slot_relic"),
                    ["ItemFactionBooster"] = language.GetTag("iatag_slot_scroll"),
                    ["ItemEnchantment"] = language.GetTag("iatag_slot_augmentation")
                };
            }

            if (!string.IsNullOrEmpty(stat) && SlotMap.ContainsKey(stat))
            {
                return SlotMap[stat];
            }

            return stat;
        }
    }
}

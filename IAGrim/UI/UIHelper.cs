using IAGrim.Theme;
using IAGrim.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI {
    class UIHelper {

        public static Action<Form, Panel> AddAndShow = (Form f, Panel p) => {
            f.TopLevel = false;
            p.Controls.Add(f);
            f.Show();
        };

        public static ComboBoxItemQuality[] QualityFilter {
            get {
                // 
                List<ComboBoxItemQuality> comboBoxItemQuality = new List<ComboBoxItemQuality>();
                comboBoxItemQuality.Add(new ComboBoxItemQuality { Text = GlobalSettings.Language.GetTag("iatag_rarity_any"), Rarity = null });
                comboBoxItemQuality.Add(new ComboBoxItemQuality { Text = GlobalSettings.Language.GetTag("iatag_rarity_yellow"), Rarity = "Yellow" });
                comboBoxItemQuality.Add(new ComboBoxItemQuality { Text = GlobalSettings.Language.GetTag("iatag_rarity_green"), Rarity = "Green" });
                comboBoxItemQuality.Add(new ComboBoxItemQuality { Text = GlobalSettings.Language.GetTag("iatag_rarity_blue"), Rarity = "Blue" });
                comboBoxItemQuality.Add(new ComboBoxItemQuality { Text = GlobalSettings.Language.GetTag("iatag_rarity_epic"), Rarity = "Epic" });
                return comboBoxItemQuality.ToArray<ComboBoxItemQuality>();
            }
        }


        public static ComboBoxItem[] SlotFilter {
            get {
                var L = GlobalSettings.Language;
                List<ComboBoxItem> slotFilter = new List<ComboBoxItem>();
                // Fill the slot dropdown
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_any"), Filter = null });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_head"), Filter = new string[] { "ArmorProtective_Head" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_hands"), Filter = new string[] { "ArmorProtective_Hands" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_feet"), Filter = new string[] { "ArmorProtective_Feet" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_legs"), Filter = new string[] { "ArmorProtective_Legs" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_chest"), Filter = new string[] { "ArmorProtective_Chest" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_belt"), Filter = new string[] { "ArmorProtective_Waist" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_medal"), Filter = new string[] { "ArmorJewelry_Medal" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_ring"), Filter = new string[] { "ArmorJewelry_Ring" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_shoulder"), Filter = new string[] { "ArmorProtective_Shoulders" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_neck"), Filter = new string[] { "ArmorJewelry_Amulet" } });

                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_weapon1h"), Filter = new string[] { "WeaponMelee_Dagger", "WeaponMelee_Mace", "WeaponMelee_Axe", "WeaponMelee_Scepter", "WeaponMelee_Sword" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_weapon2h"), Filter = new string[] { "WeaponMelee_Sword2h", "WeaponMelee_Mace2h", "WeaponMelee_Axe2h" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_weaponranged"), Filter = new string[] { "WeaponHunting_Ranged2h", "WeaponHunting_Ranged1h" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_offhand"), Filter = new string[] { "WeaponArmor_Offhand" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_shield"), Filter = new string[] { "WeaponArmor_Shield" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_component"), Filter = new string[] { "ItemRelic" } });
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_relic"), Filter = new string[] { "ItemArtifact" } });
                

                List<string> otherTable = new List<string>();
                foreach (ComboBoxItem item in slotFilter) {
                    if (item.Filter != null)
                        otherTable.AddRange(item.Filter);
                }
                slotFilter.Add(new ComboBoxItem { Text = L.GetTag("iatag_slot_other"), Filter = otherTable.ToArray<string>() });


                return slotFilter.ToArray<ComboBoxItem>();
            }
        }
    }
}

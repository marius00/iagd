using IAGrim.Theme;
using IAGrim.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IAGrim.UI
{
    class UIHelper {

        public static Action<Form, Panel> AddAndShow = (f, p) => {
            f.TopLevel = false;
            p.Controls.Add(f);
            p.Width = p.Parent.Width;
            p.Height = p.Parent.Height;
            f.Show();
        };

        public static ComboBoxItemQuality[] QualityFilter {
            get {
                List<ComboBoxItemQuality> comboBoxItemQuality = new List<ComboBoxItemQuality>
                {
                    new ComboBoxItemQuality
                    {
                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_any"),
                        Rarity = null
                    },
                    new ComboBoxItemQuality
                    {
                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_yellow"),
                        Rarity = "Yellow"
                    },
                    new ComboBoxItemQuality
                    {

                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_green"),
                        Rarity = "Green",
                    },
                    new ComboBoxItemQuality
                    {

                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_green_p1"),
                        Rarity = "Green",
                        PrefixRarity = 1
                    },
                    new ComboBoxItemQuality
                    {

                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_green_p2"),
                        Rarity = "Green",
                        PrefixRarity = 2
                    },
                    new ComboBoxItemQuality
                    {
                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_blue"),
                        Rarity = "Blue"
                    },
                    new ComboBoxItemQuality
                    {
                        Text = RuntimeSettings.Language.GetTag("iatag_rarity_epic"),
                        Rarity = "Epic"
                    }
                };
                return comboBoxItemQuality.ToArray<ComboBoxItemQuality>();
            }
        }

        public static ComboBoxItem[] SlotFilter {
            get {
                var language = RuntimeSettings.Language;
                List<ComboBoxItem> slotFilter = new List<ComboBoxItem>
                {
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_any"),
                        Filter = null

                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_armor"),
                        Filter = new[] {"ArmorProtective_Head", "ArmorProtective_Hands", "ArmorProtective_Feet", "ArmorProtective_Legs", "ArmorProtective_Chest", "ArmorProtective_Waist", "ArmorJewelry_Medal", "ArmorJewelry_Ring", "ArmorProtective_Shoulders", "ArmorJewelry_Amulet" }
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_head"),
                        Filter = new[] {"ArmorProtective_Head"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_hands"),
                        Filter = new[] {"ArmorProtective_Hands"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_feet"),
                        Filter = new[] {"ArmorProtective_Feet"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_legs"),
                        Filter = new[] {"ArmorProtective_Legs"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_chest"),
                        Filter = new[] {"ArmorProtective_Chest"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_belt"),
                        Filter = new[] {"ArmorProtective_Waist"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_medal"),
                        Filter = new[] {"ArmorJewelry_Medal"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_ring"),
                        Filter = new[] {"ArmorJewelry_Ring"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_shoulder"),
                        Filter = new[] {"ArmorProtective_Shoulders"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_neck"),
                        Filter = new[] {"ArmorJewelry_Amulet"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_weapon1h"),
                        Filter = new[]
                        {
                            "WeaponMelee_Dagger", "WeaponMelee_Mace", "WeaponMelee_Axe",
                            "WeaponMelee_Scepter", "WeaponMelee_Sword"
                        }
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_weapon2h"),
                        Filter = new[] {"WeaponMelee_Sword2h", "WeaponMelee_Mace2h", "WeaponMelee_Axe2h", "WeaponMelee_Spear2h" }
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_weaponranged1h"),
                        Filter = new[] {"WeaponHunting_Ranged1h"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_weaponranged2h"),
                        Filter = new[] {"WeaponHunting_Ranged2h"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_offhand"),
                        Filter = new[] {"WeaponArmor_Offhand"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_shield"),
                        Filter = new[] {"WeaponArmor_Shield"}
                    },
                    new ComboBoxItem
                    {
                        Text = language.GetTag("iatag_slot_relic"),
                        Filter = new[] {"ItemArtifact"}
                    }
                };

                List<string> otherTable = new List<string>();

                foreach (ComboBoxItem item in slotFilter) {
                    if (item.Filter != null)
                    {
                        otherTable.AddRange(item.Filter);
                    }  
                }

                slotFilter.Add(new ComboBoxItem
                {
                    Text = language.GetTag("iatag_slot_other"),
                    Filter = otherTable.ToArray<string>()
                });


                return slotFilter.ToArray<ComboBoxItem>();
            }
        }
    }
}

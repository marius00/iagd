using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.UI.Filters {
    public partial class Misc : UserControl {
        public Misc() {
            InitializeComponent();
        }

        public void Misc_Load(object sender, EventArgs e) {
            miscPanel.ToggleState();
        }

        public bool SocketedOnly => cbSocketed.Checked;
        public bool DuplicatesOnly => cbDuplicates.Checked;
        public bool PetBonuses => cbPetBonuses.Checked;
        public bool RecentOnly => cbRecentOnly.Checked;
        public bool GrantsSkill => cbGrantsSkill.Checked;
        public bool WithSummonerSkillOnly => cbSummonerSkill.Checked;

        public bool CraftableOnly => cbCraftable.Checked;

        public List<string[]> Filters {
            get {
                var filters = new List<string[]>();

                if (setbonus.Checked) {
                    filters.Add(new[] { "setName", "itemSetName" });
                }

                if (shieldStuff.Checked) {
                    filters.Add(new[] {
                        "blockAbsorption", "defensiveBlock", "defensiveBlockChance", "defensiveBlockModifier",
                        "defensiveBlockAmountModifier"
                    });
                }

                if (cbAttackSpeed.Checked) {
                    filters.Add(new[]
                        {"characterAttackSpeedModifier", "characterAttackSpeed", "characterTotalSpeedModifier"});
                }

                if (cbCastspeed.Checked) {
                    filters.Add(new[] {"characterSpellCastSpeedModifier", "characterTotalSpeedModifier"});
                }

                if (cbIncreaseArmor.Checked) {
                    filters.Add(new[] { "defensiveProtectionModifier" });
                }

                if (cbRunspeed.Checked) {
                    filters.Add(new[] {"characterRunSpeedModifier", "characterTotalSpeedModifier"});
                }

                if (exp.Checked) {
                    filters.Add(new[] {"characterIncreasedExperience"});
                }

                if (cbReflect.Checked) {
                    filters.Add(new[] {"defensiveReflect"});
                }

                if (health.Checked) {
                    filters.Add(new[] {"characterLifeModifier", "characterLife"});
                }

                if (cbDefense.Checked) {
                    filters.Add(new[] {"characterDefensiveAbilityModifier", "characterDefensiveAbility"});
                }

                if (cbOffensive.Checked) {
                    filters.Add(new[] {"characterOffensiveAbility", "characterOffensiveAbilityModifier"});
                }

                if (cbMasterySkills.Checked) {
                    filters.Add(new[] {"augmentMastery1", "augmentMastery2"});
                }

                if (cbPetBonuses.Checked) {
                    filters.Add(new[] {"petBonusName"});
                }

                if (cbEnergyRegen.Checked) {
                    filters.Add(new[] {"characterManaRegen", "characterManaRegenModifier"});
                }

                if (cbWeaponLifeLeech.Checked) {
                    filters.Add(new[] { "offensiveLifeLeechMin" });
                }

                if (cbDamageConversion.Checked) {
                    filters.Add(new[] { "conversionPercentage" });
                }

                if (cbCooldownReduction.Checked) {
                    filters.Add(new[] { "skillCooldownReduction" });
                }

                if (cbPhysique.Checked) {
                    filters.Add(new[] { "characterStrength", "characterStrengthModifier" });
                }

                if (cbSpirit.Checked) {
                    filters.Add(new[] { "characterIntelligence", "characterIntelligenceModifier" });
                }

                if (cbCunning.Checked) {
                    filters.Add(new[] { "characterDexterity", "characterDexterityModifier" });
                }

                return filters;
            }
        }
    }
}

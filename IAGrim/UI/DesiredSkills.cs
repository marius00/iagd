using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IAGrim.UI
{
    partial class DesiredSkills : Form {
        private readonly IItemTagDao _itemTagDao;
        private readonly Dictionary<string, FirefoxCheckBox> _classes;

        public DesiredSkills(IItemTagDao itemTagDao) {
            InitializeComponent();
            _itemTagDao = itemTagDao;
            _classes = new Dictionary<string, FirefoxCheckBox>();
        }

        private void DesiredSkills_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;

            // 3,4+n*33
            // 3,37
            // 3,70
            int cbNum = 1;
            var classtags = _itemTagDao.GetValidClassItemTags();
            
            foreach (var tag in classtags) {
                // Filter out 4 digit classes (combos)
                var cleanTag = Regex.Replace(tag.Tag, @"[^\d]", "");
                if (cleanTag.Length > 3)
                    continue;
                
                var cb = new FirefoxCheckBox {
                    Size = new Size { Height = 27, Width = 121 },
                    Tag = $"iatag_ui_{tag.Name.ToLower()}",
                    Text = tag.Name,
                    Location = new Point { X = 3, Y = 3 + cbNum*33 }
                };

                _classes[tag.Tag] = cb;
                classesPanelBox.Controls.Add(cb);

                cbNum++;
            }

            classesPanelBox.Size = new Size {
                Height = 10 + cbNum * 33,
                Width = 163
            };

            InitControlsRecursive(Controls);

            ResizeChildren();
        }

        private void ResizeChildren()
        {
            var fullWidth = Width - 36;
            panelBox1.Width = fullWidth;
            panelBox2.Width = fullWidth;
            panelBox3.Width = fullWidth;
            panelBox4.Width = fullWidth;
            classesPanelBox.Width = fullWidth;
        }

        public event EventHandler<FilterEventArgs> OnChanged;

        void InitControlsRecursive(Control.ControlCollection coll) {
            foreach (Control c in coll) {
                if (c is FirefoxCheckBox cb) {
                    cb.CheckedChanged += (sender, e) => {
                        var handler = OnChanged;
                        
                        // Only search if the user desires auto search (probably 99%)
                        if (handler != null && Properties.Settings.Default.AutoSearch)
                            handler(this, Filters);
                    };
                }
                InitControlsRecursive(c.Controls);
            }
        }
        
        /// <summary>
        /// Set all the filters to false
        /// </summary>
        public void ClearFilters() {
            ClearFilters(Controls);
        }

        private void ClearFilters(System.Windows.Forms.Control.ControlCollection coll) {
            foreach (Control c in coll) {
                if (c is FirefoxCheckBox cb) {
                    cb.Checked = false;
                }
                ClearFilters(c.Controls);
            }
        }

        public FilterEventArgs Filters =>
            new FilterEventArgs {
                Filters = OrFilters,
                PetBonuses = cbPetBonuses.Checked,
                IsRetaliation = dmgRetaliation.Checked,
                DuplicatesOnly = cbDuplicates.Checked,
                SocketedOnly = cbSocketed.Checked,
                RecentOnly = cbRecentOnly.Checked,
                DesiredClass = DesiredClasses
            };

        private List<string> DesiredClasses {
            get {
                var result = new List<string>();
                foreach (var key in _classes.Keys) {
                    if (_classes[key].Checked)
                        result.Add(key);
                }

                return result;
            }
        }

        /// <summary>
        /// Get the desired skills to filter by
        /// Where there is more than one skill, treat it as "OR"
        /// </summary>
        private List<string[]> OrFilters {
            get {
                List<string[]> filters = new List<string[]>();

                if (setbonus.Checked)
                    filters.Add(new[] { "setName", "itemSetName" });

                if (shieldStuff.Checked)
                    filters.Add(new[] { "blockAbsorption", "defensiveBlock", "defensiveBlockChance", "defensiveBlockModifier", "defensiveBlockAmountModifier" });

                // +Damage
                List<string> dmgTypes = new List<string>();
                if (dmgPhysical.Checked)
                    dmgTypes.Add("Physical");
                if (dmgPiercing.Checked)
                    dmgTypes.Add("Pierce");
                if (dmgFire.Checked) {
                    dmgTypes.Add("Fire");
                }
                if (dmgCold.Checked) {
                    dmgTypes.Add("Cold");
                }
                if (dmgLightning.Checked) {
                    dmgTypes.Add("Lightning");
                }
                if (dmgAether.Checked)
                    dmgTypes.Add("Aether");
                if (dmgVitality.Checked)
                    dmgTypes.Add("Life");
                if (dmgChaos.Checked)
                    dmgTypes.Add("Chaos");
                if (dmgPoison.Checked)
                    dmgTypes.Add("Poison");
                if (dmgElemental.Checked)
                    dmgTypes.Add("Elemental");

                if (totalDamage.Checked)
                    filters.Add(new[] { "offensiveTotalDamageModifier" });

                foreach (string damageType in dmgTypes) {
                    bool isElemental = damageType.Equals("Fire") || damageType.Equals("Cold") || damageType.Equals("Lightning");

                    if (isElemental) {
                        filters.Add(new[]{
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier",
                            string.Format("offensiveElemental", damageType), 
                            string.Format("offensiveElementalModifier", damageType),
                            $"offensiveSlow{damageType}",
                            $"offensiveSlow{damageType}Modifier"
                        });
                    }
                    else {
                        filters.Add(new[]{
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier",
                            $"offensiveSlow{damageType}",
                            $"offensiveSlow{damageType}Modifier"
                        });
                    }
                }

                // DoT damage types
                {
                    List<string> dotTypes = new List<string>();
                    if (dmgBleeding.Checked)
                        dotTypes.Add("Bleeding");
                    if (dmgTrauma.Checked)
                        dotTypes.Add("Physical");
                    if (dmgBurn.Checked)
                        dotTypes.Add("Fire");
                    if (dmgElectrocute.Checked)
                        dotTypes.Add("Lightning");
                    if (dmgVitalityDecay.Checked)
                        dotTypes.Add("Life");
                    if (dmgFrost.Checked)
                        dotTypes.Add("Cold");

                    foreach (string dot in dotTypes) {
                        filters.Add(new[]{
                            $"offensiveSlow{dot}",
                            $"offensiveSlow{dot}Modifier",
                            $"offensiveSlow{dot}ModifierChance",
                            $"offensiveSlow{dot}DurationModifier",
                            $"retaliationSlow{dot}Min",
                            $"retaliationSlow{dot}Chance",
                            $"retaliationSlow{dot}Duration",
                            $"retaliationSlow{dot}DurationMin"
                        });                        
                    }
                }

                // Resist
                List<string> resistTypes = new List<string>();

                if (resistPhysical.Checked)
                    resistTypes.Add("Physical");
                if (resistPiercing.Checked)
                    resistTypes.Add("Pierce");
                if (resistFire.Checked)
                    resistTypes.Add("Fire");
                if (resistCold.Checked)
                    resistTypes.Add("Cold");
                if (resistLightning.Checked)
                    resistTypes.Add("Lightning");
                if (resistAether.Checked)
                    resistTypes.Add("Aether");
                if (resistVitality.Checked)
                    resistTypes.Add("Life");
                if (resistChaos.Checked)
                    resistTypes.Add("Chaos");
                if (resistPoison.Checked)
                    resistTypes.Add("Poison");
                if (resistBleeding.Checked)
                    resistTypes.Add("Bleeding");
                if (resistElemental.Checked)
                    filters.Add(new[] { "defensiveElementalResistance" });

                foreach (string damageType in resistTypes) {
                    filters.Add(new[]{
                        $"defensive{damageType}",
                        $"defensive{damageType}Modifier",
                        $"defensiveSlow{damageType}",
                        $"defensiveSlow{damageType}Modifier"
                    });
                }

                // Misc
                if (cbAttackSpeed.Checked) {
                    filters.Add(new[] { "characterAttackSpeedModifier", "characterAttackSpeed", "characterTotalSpeedModifier" });
                }

                if (cbCastspeed.Checked)
                    filters.Add(new[] { "characterSpellCastSpeedModifier", "characterTotalSpeedModifier" });

                if (cbRunspeed.Checked)
                    filters.Add(new[] { "characterRunSpeedModifier", "characterTotalSpeedModifier" });

                if (exp.Checked)
                    filters.Add(new[] { "characterIncreasedExperience" });

                if (cbReflect.Checked)
                    filters.Add(new[] { "defensiveReflect" });

                if (cbLifeLeech.Checked)
                    filters.Add(new[] { "offensiveLifeLeechMin", "offensiveSlowLifeLeachMin" });

                if (health.Checked) {
                    filters.Add(new[]{"characterLifeModifier", "characterLife"});
                }

                if (cbDefense.Checked) {
                    filters.Add(new[]{"characterDefensiveAbilityModifier", "characterDefensiveAbility"});
                }

                if (cbOffensive.Checked) {
                    filters.Add(new[] { "characterOffensiveAbility", "characterOffensiveAbilityModifier" });
                }

                if (cbMasterySkills.Checked) {
                    filters.Add(new[] { "augmentMastery1", "augmentMastery2" });
                }

                if (cbPetBonuses.Checked) {
                    filters.Add(new[] { "petBonusName" });
                }

                return filters;
            }
        }

        private void DesiredSkills_Resize(object sender, EventArgs e)
        {
            ResizeChildren();
        }
    }

    class FilterEventArgs : EventArgs {
        public List<string[]> Filters { get; set; }
        public bool PetBonuses { get; set; }
        public bool IsRetaliation { get; set; }

        public bool DuplicatesOnly { get; set; }

        public bool SocketedOnly { get; set; }

        public bool RecentOnly { get; set; }

        public List<string> DesiredClass { get; set; }
    }
}

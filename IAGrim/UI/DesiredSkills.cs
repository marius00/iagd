using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IAGrim.Theme;

namespace IAGrim.UI
{
    partial class DesiredSkills : AutoSizeForm {
        private readonly IItemTagDao _itemTagDao;
        private readonly Dictionary<string, FirefoxCheckBox> _classes;

        public DesiredSkills(IItemTagDao itemTagDao)
        {
            InitializeComponent();
            _itemTagDao = itemTagDao;
            _classes = new Dictionary<string, FirefoxCheckBox>();
        }

        public FilterEventArgs Filters =>
            new FilterEventArgs
            {
                Filters = OrFilters,
                PetBonuses = cbPetBonuses.Checked,
                IsRetaliation = dmgRetaliation.Checked,
                DuplicatesOnly = cbDuplicates.Checked,
                SocketedOnly = cbSocketed.Checked,
                RecentOnly = cbRecentOnly.Checked,
                DesiredClass = DesiredClasses
            };

        public event EventHandler<FilterEventArgs> OnChanged;

        private List<string> DesiredClasses
        {
            get
            {
                var result = new List<string>();
                foreach (var key in _classes.Keys)
                {
                    if (_classes[key].Checked)
                    {
                        result.Add(key);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Get the desired skills to filter by
        /// Where there is more than one skill, treat it as "OR"
        /// </summary>
        private List<string[]> OrFilters
        {
            get
            {
                var filters = new List<string[]>();

                if (setbonus.Checked)
                {
                    filters.Add(new[] {"setName", "itemSetName"});
                }

                if (shieldStuff.Checked)
                {
                    filters.Add(new[]
                    {
                        "blockAbsorption", "defensiveBlock", "defensiveBlockChance", "defensiveBlockModifier",
                        "defensiveBlockAmountModifier"
                    });
                }

                // +Damage
                var dmgTypes = new List<string>();
                if (dmgPhysical.Checked)
                {
                    dmgTypes.Add("Physical");
                }

                if (dmgPiercing.Checked)
                {
                    dmgTypes.Add("Pierce");
                }

                if (dmgFire.Checked)
                {
                    dmgTypes.Add("Fire");
                }

                if (dmgCold.Checked)
                {
                    dmgTypes.Add("Cold");
                }

                if (dmgLightning.Checked)
                {
                    dmgTypes.Add("Lightning");
                }

                if (dmgAether.Checked)
                {
                    dmgTypes.Add("Aether");
                }

                if (dmgVitality.Checked)
                {
                    dmgTypes.Add("Life");
                }

                if (dmgChaos.Checked)
                {
                    dmgTypes.Add("Chaos");
                }

                if (dmgPoison.Checked)
                {
                    dmgTypes.Add("Poison");
                }

                if (dmgElemental.Checked)
                {
                    dmgTypes.Add("Elemental");
                }

                if (totalDamage.Checked)
                {
                    filters.Add(new[] {"offensiveTotalDamageModifier"});
                }

                foreach (var damageType in dmgTypes)
                {
                    var isElemental = damageType.Equals("Fire") || damageType.Equals("Cold") ||
                                      damageType.Equals("Lightning");

                    if (isElemental)
                    {
                        filters.Add(new[]
                        {
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier",
                            "offensiveElemental",
                            "offensiveElementalModifier",
                            $"offensiveSlow{damageType}",
                            $"offensiveSlow{damageType}Modifier"
                        });
                    }
                    else
                    {
                        filters.Add(new[]
                        {
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier",
                            $"offensiveSlow{damageType}",
                            $"offensiveSlow{damageType}Modifier"
                        });
                    }
                }

                // DoT damage types
                {
                    var dotTypes = new List<string>();
                    if (dmgBleeding.Checked)
                    {
                        dotTypes.Add("Bleeding");
                    }

                    if (dmgTrauma.Checked)
                    {
                        dotTypes.Add("Physical");
                    }

                    if (dmgBurn.Checked)
                    {
                        dotTypes.Add("Fire");
                    }

                    if (dmgElectrocute.Checked)
                    {
                        dotTypes.Add("Lightning");
                    }

                    if (dmgVitalityDecay.Checked)
                    {
                        dotTypes.Add("Life");
                    }

                    if (dmgFrost.Checked)
                    {
                        dotTypes.Add("Cold");
                    }

                    foreach (var dot in dotTypes)
                    {
                        filters.Add(new[]
                        {
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
                var resistTypes = new List<string>();

                if (resistPhysical.Checked)
                {
                    resistTypes.Add("Physical");
                }

                if (resistPiercing.Checked)
                {
                    resistTypes.Add("Pierce");
                }

                if (resistFire.Checked)
                {
                    resistTypes.Add("Fire");
                }

                if (resistCold.Checked)
                {
                    resistTypes.Add("Cold");
                }

                if (resistLightning.Checked)
                {
                    resistTypes.Add("Lightning");
                }

                if (resistAether.Checked)
                {
                    resistTypes.Add("Aether");
                }

                if (resistVitality.Checked)
                {
                    resistTypes.Add("Life");
                }

                if (resistChaos.Checked)
                {
                    resistTypes.Add("Chaos");
                }

                if (resistPoison.Checked)
                {
                    resistTypes.Add("Poison");
                }

                if (resistBleeding.Checked)
                {
                    resistTypes.Add("Bleeding");
                }

                if (resistElemental.Checked)
                {
                    filters.Add(new[] {"defensiveElementalResistance"});
                }

                foreach (var damageType in resistTypes)
                {
                    filters.Add(new[]
                    {
                        $"defensive{damageType}",
                        $"defensive{damageType}Modifier",
                        $"defensiveSlow{damageType}",
                        $"defensiveSlow{damageType}Modifier"
                    });
                }

                // Misc
                if (cbAttackSpeed.Checked)
                {
                    filters.Add(new[]
                        {"characterAttackSpeedModifier", "characterAttackSpeed", "characterTotalSpeedModifier"});
                }

                if (cbCastspeed.Checked)
                {
                    filters.Add(new[] {"characterSpellCastSpeedModifier", "characterTotalSpeedModifier"});
                }

                if (cbRunspeed.Checked)
                {
                    filters.Add(new[] {"characterRunSpeedModifier", "characterTotalSpeedModifier"});
                }

                if (exp.Checked)
                {
                    filters.Add(new[] {"characterIncreasedExperience"});
                }

                if (cbReflect.Checked)
                {
                    filters.Add(new[] {"defensiveReflect"});
                }

                if (cbLifeLeech.Checked)
                {
                    filters.Add(new[] {"offensiveLifeLeechMin", "offensiveSlowLifeLeachMin"});
                }

                if (health.Checked)
                {
                    filters.Add(new[] {"characterLifeModifier", "characterLife"});
                }

                if (cbDefense.Checked)
                {
                    filters.Add(new[] {"characterDefensiveAbilityModifier", "characterDefensiveAbility"});
                }

                if (cbOffensive.Checked)
                {
                    filters.Add(new[] {"characterOffensiveAbility", "characterOffensiveAbilityModifier"});
                }

                if (cbMasterySkills.Checked)
                {
                    filters.Add(new[] {"augmentMastery1", "augmentMastery2"});
                }

                if (cbPetBonuses.Checked)
                {
                    filters.Add(new[] {"petBonusName"});
                }

                return filters;
            }
        }

        /// <summary>
        /// Set all the filters to false
        /// </summary>
        public void ClearFilters()
        {
            ClearFilters(Controls);
        }

        private void DesiredSkills_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;

            // 3,4+n*33
            // 3,37
            // 3,70
            var cbNum = 1;
            var classTags = _itemTagDao.GetValidClassItemTags()
                .Where(entry => Regex.Replace(entry.Tag, @"[^\d]", "").Length <= 3) // Filter out 4 digit classes (combo classes)
                .ToList();

            foreach (var tag in classTags) {

                var cb = new FirefoxCheckBox {
                    Size = new Size {Height = 27, Width = 121},
                    Tag = $"iatag_ui_{tag.Name.ToLowerInvariant()}",
                    Text = tag.Name,
                    Location = new Point {X = 3, Y = 3 + cbNum * 33}
                };

                _classes[tag.Tag] = cb;
                classesPanelBox.Controls.Add(cb);

                cbNum++;
            }

            classesPanelBox.Size = new Size
            {
                Height = 10 + cbNum * 33,
                Width = classesPanelBox.Size.Width
            };

            InitControlsRecursive(Controls);

            dotPanel.ToggleState();
            miscPanel.ToggleState();
        }

        private void InitControlsRecursive(Control.ControlCollection coll)
        {
            foreach (Control c in coll)
            {
                if (c is FirefoxCheckBox cb)
                {
                    cb.CheckedChanged += (sender, e) =>
                    {
                        var handler = OnChanged;

                        // Only search if the user desires auto search (probably 99%)
                        handler?.Invoke(this, Filters);
                    };
                }

                InitControlsRecursive(c.Controls);
            }
        }

        private void ClearFilters(Control.ControlCollection coll)
        {
            foreach (Control c in coll)
            {
                if (c is FirefoxCheckBox cb)
                {
                    cb.Checked = false;
                }

                ClearFilters(c.Controls);
            }
        }
    }
}

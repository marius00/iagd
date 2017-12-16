using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IAGrim.UI {
    partial class DesiredSkills : Form {
        private readonly IDatabaseItemDao databaseItemDao;
        private readonly Dictionary<String, FirefoxCheckBox> classes;

        public DesiredSkills(IDatabaseItemDao databaseItemDao) {
            InitializeComponent();
            this.databaseItemDao = databaseItemDao;
            this.classes = new Dictionary<string, FirefoxCheckBox>();
        }


        private void DesiredSkills_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            // 3,4+n*33
            // 3,37
            // 3,70
            int cbNum = 1;
            var classtags = databaseItemDao.GetValidClassItemTags();
            
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

                classes[tag.Tag] = cb;
                classesPanelBox.Controls.Add(cb);

                cbNum++;
            }

            classesPanelBox.Size = new Size {
                Height = 10 + cbNum * 33,
                Width = 163
            };


            initControlsRecursive(this.Controls);
        }

        public event EventHandler<FilterEventArgs> OnChanged;


        void initControlsRecursive(System.Windows.Forms.Control.ControlCollection coll) {
            foreach (Control c in coll) {
                FirefoxCheckBox cb = c as FirefoxCheckBox;
                if (cb != null) {
                    cb.CheckedChanged += (sender, e) => {
                        var handler = OnChanged;
                        
                        // Only search if the user desires auto search (probably 99%)
                        if (handler != null && Properties.Settings.Default.AutoSearch)
                            handler(this, Filters);
                    };
                }
                initControlsRecursive(c.Controls);
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
                FirefoxCheckBox cb = c as FirefoxCheckBox;
                if (cb != null) {
                    cb.Checked = false;
                }
                ClearFilters(c.Controls);
            }
        }

        public FilterEventArgs Filters {
            get {
                return new FilterEventArgs {
                    Filters = OrFilters,
                    PetBonuses = cbPetBonuses.Checked,
                    IsRetaliation = dmgRetaliation.Checked,
                    DuplicatesOnly = cbDuplicates.Checked,
                    SocketedOnly = cbSocketed.Checked,
                    DesiredClass = DesiredClasses
                };
            }
        }

        private List<string> DesiredClasses {
            get {
                var result = new List<string>();
                foreach (var key in classes.Keys) {
                    if (classes[key].Checked)
                        result.Add(key);
                }/*
                if (cbClassSoldier.Checked)
                    result.Add("class01");
                if (cbClassDemo.Checked)
                    result.Add("class02");
                if (cbClassOccultist.Checked)
                    result.Add("class03");
                if (cbClassNightblade.Checked)
                    result.Add("class04");
                if (cbClassArcanist.Checked)
                    result.Add("class05");
                if (cbClassShaman.Checked)
                    result.Add("class06");
*/

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
                    filters.Add(new string[] { "setName", "itemSetName" });

                if (shieldStuff.Checked)
                    filters.Add(new string[] { "blockAbsorption", "defensiveBlock", "defensiveBlockChance", "defensiveBlockModifier", "defensiveBlockAmountModifier" });

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
                    filters.Add(new string[] { "offensiveTotalDamageModifier" });


                foreach (string damageType in dmgTypes) {
                    bool isElemental = damageType.Equals("Fire") || damageType.Equals("Cold") || damageType.Equals("Lightning");

                    if (isElemental) {
                        filters.Add(new string[]{
                            string.Format("offensive{0}", damageType), 
                            string.Format("offensive{0}Modifier", damageType),
                            string.Format("offensiveElemental", damageType), 
                            string.Format("offensiveElementalModifier", damageType),                           
                            string.Format("offensiveSlow{0}", damageType), 
                            string.Format("offensiveSlow{0}Modifier", damageType)
                        });
                    }
                    else {
                        filters.Add(new string[]{
                            string.Format("offensive{0}", damageType), 
                            string.Format("offensive{0}Modifier", damageType),
                            string.Format("offensiveSlow{0}", damageType), 
                            string.Format("offensiveSlow{0}Modifier", damageType)
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
                        filters.Add(new string[]{
                            string.Format("offensiveSlow{0}", dot), 
                            string.Format("offensiveSlow{0}Modifier", dot),
                            string.Format("offensiveSlow{0}ModifierChance", dot),
                            string.Format("offensiveSlow{0}DurationModifier", dot),
                            string.Format("retaliationSlow{0}Min", dot),
                            string.Format("retaliationSlow{0}Chance", dot),
                            string.Format("retaliationSlow{0}Duration", dot),
                            string.Format("retaliationSlow{0}DurationMin", dot)
                            
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
                    filters.Add(new string[] { "defensiveElementalResistance" });

                foreach (string damageType in resistTypes) {
                    filters.Add(new string[]{
                        string.Format("defensive{0}", damageType),
                        string.Format("defensive{0}Modifier", damageType),
                        string.Format("defensiveSlow{0}", damageType),
                        string.Format("defensiveSlow{0}Modifier", damageType)
                    });
                    
                }


                // Misc
                if (cbAttackSpeed.Checked) {
                    filters.Add(new string[] { "characterAttackSpeedModifier", "characterAttackSpeed", "characterTotalSpeedModifier" });
                }

                if (cbCastspeed.Checked)
                    filters.Add(new string[] { "characterSpellCastSpeedModifier", "characterTotalSpeedModifier" });

                if (cbRunspeed.Checked)
                    filters.Add(new string[] { "characterRunSpeedModifier", "characterTotalSpeedModifier" });

                if (exp.Checked)
                    filters.Add(new string[] { "characterIncreasedExperience" });

                if (cbReflect.Checked)
                    filters.Add(new string[] { "defensiveReflect" });

                if (cbLifeLeech.Checked)
                    filters.Add(new string[] { "offensiveLifeLeechMin", "offensiveSlowLifeLeachMin" });

                if (health.Checked) {
                    filters.Add(new string[]{"characterLifeModifier", "characterLife"});
                }

                if (cbDefense.Checked) {
                    filters.Add(new string[]{"characterDefensiveAbilityModifier", "characterDefensiveAbility"});
                }

                if (cbOffensive.Checked) {
                    filters.Add(new string[] { "characterOffensiveAbility", "characterOffensiveAbilityModifier" });
                }

                if (cbMasterySkills.Checked) {
                    filters.Add(new string[] { "augmentMastery1", "augmentMastery2" });
                }

                if (cbPetBonuses.Checked) {
                    filters.Add(new string[] { "petBonusName" });
                }



                return filters;
            }
        }

        private void panelBox2_Click(object sender, EventArgs e) {

        }

        private void dmgBleeding_CheckedChanged(object sender, EventArgs e) {

        }

        private void dmgFrost_CheckedChanged(object sender, EventArgs e) {

        }

    }

    class FilterEventArgs : EventArgs {
        public List<string[]> Filters { get; set; }
        public bool PetBonuses { get; set; }
        public bool IsRetaliation { get; set; }

        public bool DuplicatesOnly { get; set; }

        public bool SocketedOnly { get; set; }

        public List<string> DesiredClass { get; set; }
    }
}

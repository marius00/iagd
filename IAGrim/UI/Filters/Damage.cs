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
    public partial class Damage : UserControl {
        public Damage() {
            InitializeComponent();
        }

        public bool RetaliationDamage => dmgRetaliation.Checked;

        public List<string[]> Filters {
            get {
                var filters = new List<string[]>();

                // +Damage
                var dmgTypes = new List<string>();
                if (dmgPhysical.Checked) {
                    dmgTypes.Add("Physical");
                }

                if (dmgPiercing.Checked) {
                    dmgTypes.Add("Pierce");
                }

                if (dmgFire.Checked) {
                    dmgTypes.Add("Fire");
                }

                if (dmgCold.Checked) {
                    dmgTypes.Add("Cold");
                }

                if (dmgLightning.Checked) {
                    dmgTypes.Add("Lightning");
                }

                if (dmgAether.Checked) {
                    dmgTypes.Add("Aether");
                }

                if (dmgVitality.Checked) {
                    dmgTypes.Add("Life");
                }

                if (dmgChaos.Checked) {
                    dmgTypes.Add("Chaos");
                }

                if (dmgAcid.Checked) {
                    dmgTypes.Add("Poison");
                }

                if (dmgElemental.Checked) {
                    dmgTypes.Add("Elemental");
                }

                if (totalDamage.Checked) {
                    filters.Add(new[] { "offensiveTotalDamageModifier" });
                }


                foreach (var damageType in dmgTypes) {
                    var isElemental = damageType.Equals("Fire") || damageType.Equals("Cold") ||
                                      damageType.Equals("Lightning");

                    if (isElemental) {
                        filters.Add(new[]
                        {
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier",
                            "offensiveElemental",
                            "offensiveElementalModifier"
                        });
                    }
                    else {
                        filters.Add(new[]
                        {
                            $"offensive{damageType}",
                            $"offensive{damageType}Modifier"
                        });
                    }
                }

                return filters;
            }
        }
    }
}

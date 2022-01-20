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
    public partial class Resistances : UserControl {
        public Resistances() {
            InitializeComponent();
        }

        public List<string[]> Filters {
            get {
                // Resist
                var resistTypes = new List<string>();

                if (resistPhysical.Checked) {
                    resistTypes.Add("Physical");
                }

                if (resistPiercing.Checked) {
                    resistTypes.Add("Pierce");
                }

                if (resistFire.Checked) {
                    resistTypes.Add("Fire");
                }

                if (resistCold.Checked) {
                    resistTypes.Add("Cold");
                }

                if (resistLightning.Checked) {
                    resistTypes.Add("Lightning");
                }

                if (resistAether.Checked) {
                    resistTypes.Add("Aether");
                }

                if (resistVitality.Checked) {
                    resistTypes.Add("Life");
                }

                if (resistChaos.Checked) {
                    resistTypes.Add("Chaos");
                }

                if (resistPoison.Checked) {
                    resistTypes.Add("Poison");
                }

                if (resistBleeding.Checked) {
                    resistTypes.Add("Bleeding");
                }

                if (resistStun.Checked) {
                    resistTypes.Add("Stun");
                }

                var filters = new List<string[]>();
                if (resistElemental.Checked) {
                    filters.Add(new[] { "defensiveElementalResistance" });
                }

                foreach (var damageType in resistTypes) {
                    filters.Add(new[]
                    {
                        $"defensive{damageType}",
                        $"defensive{damageType}Modifier",
                        $"defensiveSlow{damageType}",
                        $"defensiveSlow{damageType}Modifier"
                    });
                }


                if (resistSlow.Checked) {
                    filters.Add(new []{"defensiveTotalSpeedResistance"});
                }
                return filters;
            }
        }
    }
}

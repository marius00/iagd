using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IAGrim.UI.Filters {
    partial class DamageOverTimeFilter : UserControl {
        public DamageOverTimeFilter() {
            InitializeComponent();
            Load += DamageOverTimeFilter_Load;
        }

        public void DamageOverTimeFilter_Load(object sender, EventArgs e) {
            dotPanel.ToggleState();
        }

        public List<string[]> Filters {
            get {
                var dotTypes = new List<string>();
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

                if (dmgPoison.Checked)
                    dotTypes.Add("Poison");

                var filters = new List<string[]>();
                foreach (var dot in dotTypes)
                    filters.Add(new[] {
                        $"offensiveSlow{dot}",
                        $"offensiveSlow{dot}Modifier",
                        $"offensiveSlow{dot}ModifierChance",
                        $"offensiveSlow{dot}DurationModifier",
                        $"retaliationSlow{dot}Min",
                        $"retaliationSlow{dot}Chance",
                        $"retaliationSlow{dot}Duration",
                        $"retaliationSlow{dot}DurationMin"
                    });

                if (dmgLifeLeech.Checked)
                    filters.Add(new[] {"offensiveLifeLeechMin", "offensiveSlowLifeLeachMin"});

                return filters;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IAGrim.Services.ItemStats;

namespace IAGrim.UI.Filters {
    partial class DamageOverTimeFilter : UserControl {
        public DamageOverTimeFilter() {
            InitializeComponent();
            FirefoxCheckBox.EnableNumericFilters(this);
            Load += DamageOverTimeFilter_Load;
        }

        public void DamageOverTimeFilter_Load(object? sender, EventArgs e) {
            dotPanel.ToggleState();
        }

        // Selected DoT checkboxes paired with their stat fields, built once so both the plain "stat exists"
        // filters and the per-checkbox numeric filters derive from the same mapping.
        private List<(FirefoxCheckBox cb, string[] fields)> SelectedStatGroups() {
            var groups = new List<(FirefoxCheckBox, string[])>();

            var dotTypes = new[] {
                (dmgBleeding, "Bleeding"),
                (dmgTrauma, "Physical"),
                (dmgBurn, "Fire"),
                (dmgElectrocute, "Lightning"),
                (dmgVitalityDecay, "Life"),
                (dmgFrost, "Cold"),
                (dmgPoison, "Poison"),
            };

            foreach (var (cb, dot) in dotTypes) {
                if (!cb.Checked)
                    continue;

                groups.Add((cb, new[] {
                    $"offensiveSlow{dot}",
                    $"offensiveSlow{dot}Modifier",
                    $"offensiveSlow{dot}ModifierChance",
                    $"offensiveSlow{dot}DurationModifier",
                    $"retaliationSlow{dot}Min",
                    $"retaliationSlow{dot}Chance",
                    $"retaliationSlow{dot}Duration",
                    $"retaliationSlow{dot}DurationMin"
                }));
            }

            if (dmgLifeLeech.Checked)
                groups.Add((dmgLifeLeech, new[] {"offensiveLifeLeechMin", "offensiveSlowLifeLeachMin"}));

            return groups;
        }

        public List<string[]> Filters => SelectedStatGroups().Select(g => g.fields).ToList();

        public List<StatValueFilter> NumericFilters => FilterBuilder.From(SelectedStatGroups());
    }
}
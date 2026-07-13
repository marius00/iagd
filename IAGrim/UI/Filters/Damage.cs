using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IAGrim.Services.ItemStats;

namespace IAGrim.UI.Filters {
    public partial class Damage : UserControl {
        public Damage() {
            InitializeComponent();
            FirefoxCheckBox.EnableNumericFilters(this);
        }

        public bool RetaliationDamage => dmgRetaliation.Checked;

        // The selected stat checkboxes paired with the stat fields they contribute, built once so both the
        // plain "stat exists" filters and the per-checkbox numeric filters derive from the same mapping.
        private List<(FirefoxCheckBox cb, string[] fields)> SelectedStatGroups() {
            var groups = new List<(FirefoxCheckBox, string[])>();

            if (totalDamage.Checked)
                groups.Add((totalDamage, new[] {"offensiveTotalDamageModifier"}));

            var damageTypes = new[] {
                (dmgPhysical, "Physical"),
                (dmgPiercing, "Pierce"),
                (dmgFire, "Fire"),
                (dmgCold, "Cold"),
                (dmgLightning, "Lightning"),
                (dmgAether, "Aether"),
                (dmgVitality, "Life"),
                (dmgChaos, "Chaos"),
                (dmgAcid, "Poison"),
                (dmgElemental, "Elemental"),
            };

            foreach (var (cb, damageType) in damageTypes) {
                if (!cb.Checked)
                    continue;

                var isElemental = damageType == "Fire" || damageType == "Cold" || damageType == "Lightning";

                var fields = isElemental
                    ? new[] {
                        $"offensive{damageType}",
                        $"offensive{damageType}Modifier",
                        "offensiveElemental",
                        "offensiveElementalModifier"
                    }
                    : new[] {
                        $"offensive{damageType}",
                        $"offensive{damageType}Modifier"
                    };

                groups.Add((cb, fields));
            }

            return groups;
        }

        public List<string[]> Filters => SelectedStatGroups().Select(g => g.fields).ToList();

        public List<StatValueFilter> NumericFilters => FilterBuilder.From(SelectedStatGroups());
    }
}
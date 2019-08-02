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
        private readonly Filters.Misc _miscFilter = new Filters.Misc();
        private readonly Filters.Damage _damageFilter = new Filters.Damage();
        private readonly Filters.DamageOverTimeFilter _dotFilter = new Filters.DamageOverTimeFilter();
        private readonly Filters.Resistances _resistanceFilters = new Filters.Resistances();
        private readonly Filters.Classes _classesFilters;

        public DesiredSkills(IItemTagDao itemTagDao) {
            InitializeComponent();

            var damageFilterControl = _damageFilter.Controls[0];
            damageFilterControl.Location = new Point(12, 9);
            Controls.Add(damageFilterControl);

            var dotFilterControl = _dotFilter.Controls[0];
            dotFilterControl.Location = new Point(12, 451);
            Controls.Add(dotFilterControl);

            var miscFilterControl = _miscFilter.Controls[0];
            miscFilterControl.Location = new Point(12, 723);
            Controls.Add(miscFilterControl);

            var resistanceFilterControl = _resistanceFilters.Controls[0];
            resistanceFilterControl.Location = new Point(12, 1275);
            Controls.Add(resistanceFilterControl);

            // Classes
            var classTags = itemTagDao.GetValidClassItemTags()
                .Where(entry => Regex.Replace(entry.Tag, @"[^\d]", "").Length <= 3) // Filter out 4 digit classes (combo classes)
                .ToList();

            _classesFilters = new Filters.Classes(classTags);

            var classesFilterControl = _classesFilters.Controls[0];
            classesFilterControl.Location = new Point(12, 1693);
            Controls.Add(classesFilterControl);
        }

        public FilterEventArgs Filters =>
            new FilterEventArgs
            {
                Filters = OrFilters,
                PetBonuses = _miscFilter.PetBonuses,
                IsRetaliation = _damageFilter.RetaliationDamage,
                DuplicatesOnly = _miscFilter.DuplicatesOnly,
                SocketedOnly = _miscFilter.SocketedOnly,
                RecentOnly = _miscFilter.RecentOnly,
                DesiredClass = _classesFilters.DesiredClasses,
                GrantsSkill = _miscFilter.GrantsSkill,
                WithSummonerSkillOnly = _miscFilter.WithSummonerSkillOnly
            };

        public event EventHandler<FilterEventArgs> OnChanged;


        /// <summary>
        /// Get the desired skills to filter by
        /// Where there is more than one skill, treat it as "OR"
        /// </summary>
        private List<string[]> OrFilters
        {
            get
            {
                var filters = new List<string[]>();

                filters.AddRange(_damageFilter.Filters);
                filters.AddRange(_dotFilter.Filters);
                filters.AddRange(_resistanceFilters.Filters);
                filters.AddRange(_miscFilter.Filters);

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
            _dotFilter.DamageOverTimeFilter_Load(sender, e);
            _miscFilter.Misc_Load(sender, e);
            _classesFilters.Classes_Load(sender, e);

            InitControlsRecursive(Controls);

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

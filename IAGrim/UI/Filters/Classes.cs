using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IAGrim.Database;

namespace IAGrim.UI.Filters {
    public partial class Classes : UserControl {
        private readonly Dictionary<string, FirefoxCheckBox> _classes;
        private readonly IList<ItemTag> _classTags;

        public Classes(IList<ItemTag> classTags) {
            InitializeComponent();
            _classes = new Dictionary<string, FirefoxCheckBox>();
            _classTags = classTags;
            Load += Classes_Load;
        }

        public List<string> DesiredClasses {
            get {
                return _classes.Keys.Where(key => _classes[key].Checked).ToList();
            }
        }

        public void Classes_Load(object sender, EventArgs e) {
            // 3,4+n*33
            // 3,37
            // 3,70
            var cbNum = 1;

            // Handle hardcoded classes
            foreach (var c in classesPanelBox.Controls) {
                if (c is FirefoxCheckBox cb) {
                    cbNum++;
                }
            }

            // TODO: Localize hardcoded classes and skip adding them if they exist
            _classes["class01"] = cbSoldier;
            _classes["class02"] = cbDemolitionist;
            _classes["class03"] = cbOccultist;
            _classes["class04"] = cbNightblade;
            _classes["class05"] = cbArcanist;
            _classes["class06"] = cbShaman;
            _classes["class07"] = cbInquisitor;
            _classes["class08"] = cbNecromancer;
            _classes["class09"] = cbOathkeeper;

            // Hardcoded classes from the base game -- Helps a bit with 4k scaling to not create these dynamically.
            var prefilled = new[] {
                "class01", "class02", "class03", "class04", "class05",
                "class06", "class07", "class08", "class09"
            };
            int yOffsetHeight = cbDemolitionist.Location.Y - cbSoldier.Location.Y;
            int cbHeight = cbDemolitionist.Height;

            foreach (var tag in _classTags) {
                var translationTag = $"{tag.Tag.ToLowerInvariant()}";

                if (!prefilled.Contains(translationTag)) {
                    var cb = new FirefoxCheckBox {
                        Size = new Size {Height = cbHeight, Width = classesPanelBox.Size.Width - 15},
                        Tag = translationTag,
                        Text = tag.Name,
                        Location = new Point {X = 3, Y = 3 + cbNum * yOffsetHeight}
                    };

                    _classes[tag.Tag] = cb;
                    classesPanelBox.Controls.Add(cb);
                    cbNum++;
                }
                else if (_classes.ContainsKey(tag.Tag)) {
                    _classes[tag.Tag].Text = tag.Name;
                }
            }

            classesPanelBox.Size = new Size {
                Height = 10 + cbNum * yOffsetHeight,
                Width = classesPanelBox.Size.Width
            };
        }
    }
}
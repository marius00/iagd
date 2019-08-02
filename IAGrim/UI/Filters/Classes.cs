using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Database;

namespace IAGrim.UI.Filters {
    public partial class Classes : UserControl {
        private readonly Dictionary<string, FirefoxCheckBox> _classes;
        private readonly IList<ItemTag> _classTags;

        public Classes(IList<ItemTag> classTags) {
            InitializeComponent();
            _classes = new Dictionary<string, FirefoxCheckBox>();
            this._classTags = classTags;
            this.Load += Classes_Load;
        }

        public List<string> DesiredClasses {
            get {
                var result = new List<string>();
                foreach (var key in _classes.Keys) {
                    if (_classes[key].Checked) {
                        result.Add(key);
                    }
                }

                return result;
            }
        }

        public void Classes_Load(object sender, EventArgs e) {
            // 3,4+n*33
            // 3,37
            // 3,70
            var cbNum = 1;

            foreach (var tag in _classTags) {

                var cb = new FirefoxCheckBox {
                    Size = new Size { Height = 27, Width = 121 },
                    Tag = $"iatag_ui_{tag.Name.ToLowerInvariant()}",
                    Text = tag.Name,
                    Location = new Point { X = 3, Y = 3 + cbNum * 33 }
                };

                _classes[tag.Tag] = cb;
                classesPanelBox.Controls.Add(cb);

                cbNum++;
            }

            classesPanelBox.Size = new Size {
                Height = 10 + cbNum * 33,
                Width = classesPanelBox.Size.Width
            };
        }
    }
}

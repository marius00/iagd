using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using IAGrim.Theme;

namespace IAGrim.UI {
    class DarkMode {
        struct ColorSet {
            public Color? BackColor;
            public Color? ForeColor;
            public Color? LinkColor;
            public Color? HoverColor;
            public Color? HeaderColor;
            public Color? HoverForeColor;
        };

        readonly Color _darkBackgroundColor = Color.FromArgb(27, 40, 56);
        readonly Color _darkForeColor = Color.White;

        private readonly Dictionary<Type, ColorSet> _darkColors = new Dictionary<Type, ColorSet>();
        private Dictionary<Type, ColorSet> _regularColors = null;
        private bool _isLightMode = true;

        public DarkMode() {
            _darkColors[typeof(TextBox)] = new ColorSet {
                BackColor = _darkBackgroundColor,
                ForeColor = Color.LightGray
            };

            _darkColors[typeof(TabControl)] = _darkColors[typeof(TextBox)];
            _darkColors[typeof(TabPage)] = _darkColors[typeof(TextBox)];
            _darkColors[typeof(Label)] = _darkColors[typeof(TextBox)];
            _darkColors[typeof(Panel)] = _darkColors[typeof(TextBox)];

            _darkColors[typeof(FirefoxCheckBox)] = new ColorSet {
                ForeColor = _darkForeColor
            };

            _darkColors[typeof(FirefoxRadioButton)] = new ColorSet {
                BackColor = _darkBackgroundColor
            };

            _darkColors[typeof(GroupBox)] = new ColorSet {
                BackColor = _darkBackgroundColor
            };

            _darkColors[typeof(FirefoxButton)] = new ColorSet {
                BackColor = Color.FromArgb(29, 55, 75),
                ForeColor = Color.FromArgb(102, 192, 239),
                HoverColor = Color.FromArgb(111, 171, 200),
                HoverForeColor = Color.FromArgb(52, 11, 139)
            };

            _darkColors[typeof(PanelBox)] = new ColorSet {
                BackColor = _darkBackgroundColor,
                ForeColor = _darkForeColor,
                HeaderColor = Color.FromArgb(38, 64, 84)
            };
            _darkColors[typeof(CollapseablePanelBox)] = _darkColors[typeof(PanelBox)];

            _darkColors[typeof(LinkLabel)] = new ColorSet {
                BackColor = _darkBackgroundColor,
                ForeColor = Color.FromArgb(102, 192, 239),
                LinkColor = Color.FromArgb(102, 192, 239)
            };

            _darkColors[typeof(Control)] = new ColorSet {
                BackColor = _darkBackgroundColor,
                ForeColor = _darkForeColor
            };
        }

        public void Activate(Control root) {
            if (_regularColors == null) {
                _regularColors = new Dictionary<Type, ColorSet>();
                FetchRegularColors(root.Controls);
                _regularColors[typeof(CollapseablePanelBox)] = _regularColors[typeof(PanelBox)];

                _regularColors[typeof(Control)] = new ColorSet {
                    BackColor = new Control().BackColor,
                    ForeColor = new Control().ForeColor
                };
            }

            _isLightMode = !_isLightMode;

            It(root.Controls, _isLightMode ? _regularColors : _darkColors);
        }

        private static void It(Control.ControlCollection collection, Dictionary<Type, ColorSet> colorSet) {
            foreach (Control control in collection) {
                It(control.Controls, colorSet);

                FirefoxButton button = control as FirefoxButton;
                PanelBox pb = control as PanelBox;
                LinkLabel linkLabel = control as LinkLabel;

                if (button != null && colorSet.ContainsKey(button.GetType())) {
                    var colorset = colorSet[button.GetType()];
                    button.BackColor = colorset.BackColor.Value;
                    button.BackColorOverride = colorset.BackColor.Value;
                    button.ForeColor = colorset.ForeColor.Value;
                    button.HoverColor = colorset.HoverColor.Value;
                    button.HoverForeColor = colorset.HoverForeColor.Value;
                }
                else if (pb != null && colorSet.ContainsKey(pb.GetType())) {
                    var colorset = colorSet[pb.GetType()];
                    pb.BackColor = colorset.BackColor.Value;
                    pb.ForeColor = colorset.ForeColor.Value;
                    pb.HeaderColor = colorset.HeaderColor.Value;
                }
                else if (linkLabel != null && colorSet.ContainsKey(linkLabel.GetType())) {
                    var colorset = colorSet[linkLabel.GetType()];
                    linkLabel.BackColor = colorset.BackColor.Value;
                    linkLabel.ForeColor = colorset.ForeColor.Value;
                    linkLabel.LinkColor = colorset.LinkColor.Value;
                }
                else {
                    if (colorSet.ContainsKey(control.GetType())) {
                        var colorset = colorSet[control.GetType()];
                        control.BackColor = colorset.BackColor ?? control.BackColor;
                        control.ForeColor = colorset.ForeColor ?? control.ForeColor;
                    }
                    else {
                        var colorset = colorSet[typeof(Control)];
                        control.BackColor = colorset.BackColor ?? control.BackColor;
                        control.ForeColor = colorset.ForeColor ?? control.ForeColor;
                    }
                }

                control.Invalidate();
            }
        }


        private void FetchRegularColors(Control.ControlCollection collection) {
            foreach (Control control in collection) {
                FetchRegularColors(control.Controls);

                FirefoxButton button = control as FirefoxButton;
                PanelBox pb = control as PanelBox;
                LinkLabel linkLabel = control as LinkLabel;

                if (button != null) {
                    _regularColors[typeof(FirefoxButton)] = new ColorSet {
                        BackColor = button.BackColor,
                        ForeColor = button.ForeColor,
                        HoverColor = button.HoverColor,
                        HoverForeColor = button.HoverForeColor
                    };
                }
                else if (pb != null) {
                    _regularColors[typeof(PanelBox)] = new ColorSet {
                        BackColor = pb.BackColor,
                        ForeColor = pb.ForeColor,
                        HeaderColor = pb.HeaderColor
                    };
                }
                else if (linkLabel != null) {
                    _regularColors[typeof(LinkLabel)] = new ColorSet {
                        BackColor = linkLabel.BackColor,
                        ForeColor = linkLabel.ForeColor,
                        LinkColor = linkLabel.LinkColor
                    };
                }
                else {
                    _regularColors[control.GetType()] = new ColorSet {
                        BackColor = control.BackColor,
                        ForeColor = control.ForeColor
                    };
                }
            }
        }
    }
}
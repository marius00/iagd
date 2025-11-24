using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IAGrim.Theme;

namespace IAGrim.UI {
    class DarkMode {
        private readonly Control _root;
        struct ColorSet {
            public Color? BackColor;
            public Color? ForeColor;
            public Color? LinkColor;
            public Color? HoverColor;
            public Color? HeaderColor;
            public Color? HoverForeColor;
        };

        readonly Color _darkBackgroundColor = Color.FromArgb(0x23, 0x22, 0x20); // Background (sidebar etc)
        readonly Color _darkForeColor = Color.FromArgb(0xb0, 0xb0, 0xb0); // Text color
        readonly Color _semiDarkColor = Color.FromArgb(0x33, 0x33, 0x33); // Background in headers etc



        private readonly Dictionary<Type, ColorSet> _darkColors = new Dictionary<Type, ColorSet>();
        private Dictionary<Type, ColorSet> _regularColors = null;
        private bool _isLightMode = true;

        public DarkMode(Control root) {
            
            _root = root;
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
                ForeColor = Color.LightGray,
                BackColor = _darkBackgroundColor
            };

            _darkColors[typeof(FirefoxButton)] = new ColorSet {
                BackColor = _semiDarkColor,
                ForeColor = _darkForeColor,
                HoverColor = _darkBackgroundColor,
                HoverForeColor = _darkForeColor,
            };

            _darkColors[typeof(PanelBox)] = new ColorSet {
                BackColor = _darkBackgroundColor,
                ForeColor = _darkForeColor,
                HeaderColor = _semiDarkColor,
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


        public void Activate() {
            if (_regularColors == null) {
                _regularColors = new Dictionary<Type, ColorSet>();
                FetchRegularColors(_root.Controls);
                _regularColors[typeof(CollapseablePanelBox)] = _regularColors[typeof(PanelBox)];

                _regularColors[typeof(Control)] = new ColorSet {
                    BackColor = new Control().BackColor,
                    ForeColor = new Control().ForeColor
                };
            }

            _isLightMode = !_isLightMode;

            var colorSet = _isLightMode ? _regularColors : _darkColors;
            HandleControl(_root, colorSet);
            It(_root.Controls, colorSet);

            Application.SetColorMode(_isLightMode ? SystemColorMode.System : SystemColorMode.Dark);
        }

        private void HandleControl(Control control, Dictionary<Type, ColorSet> colorSet) {
            FirefoxButton button = control as FirefoxButton;
            PanelBox pb = control as PanelBox;
            LinkLabel linkLabel = control as LinkLabel;
            FirefoxCheckBox cb = control as FirefoxCheckBox;

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

            if (cb != null) {
                cb.IsDarkMode = !_isLightMode;
            }

            control.Refresh();
            
        }

        private void It(Control.ControlCollection collection, Dictionary<Type, ColorSet> colorSet) {
            foreach (Control control in collection) {
                It(control.Controls, colorSet);
                HandleControl(control, colorSet);
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
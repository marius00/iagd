using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CefSharp.DevTools.HeadlessExperimental;
using IAGrim.Settings;
namespace IAGrim.UI.Misc {
    /// <summary>
    /// Responsible for persisting the window size and location across runs.
    /// </summary>
    public class WindowSizeManager {
        private Form _form;
        private readonly SettingsService _settingsService;

        public WindowSizeManager(Form form, SettingsService settingsService) {
            _form = form;
            _settingsService = settingsService;
            _form.FormClosing += _form_FormClosing;

            RestoreWindowPosition();
        }

        private void RestoreWindowPosition() {
            var obj = _settingsService.GetLocal().WindowPositionSettings;
            if (obj == null) return;


            if (!_settingsService.GetLocal().StartMinimized) {
                var screen = Screen.AllScreens.FirstOrDefault(scr => scr.DeviceName == obj.Display);
                if (screen != null) {
                    _form.StartPosition = FormStartPosition.Manual;
                    var bounds = screen.Bounds;

                    var top = obj.Top != null ? Math.Max(0, Math.Min(obj.Top.Value, Screen.FromControl(_form).Bounds.Height - 100)) : _form.Top;
                    var left = obj.Left != null ? Math.Max(0, Math.Min(obj.Left.Value, Screen.FromControl(_form).Bounds.Width - 100)) : _form.Left;
                    var width = obj.Width ?? _form.Width;
                    var height = obj.Height ?? _form.Height;
                    _form.SetBounds(bounds.X + left, bounds.Y + top, width, height);

                    _form.WindowState = (FormWindowState)obj.State;
                    return;

                }

                _form.WindowState = (FormWindowState) obj.State;
            }
                
            if (obj.Width != null && obj.Height != null) {
                _form.Size = new Size(obj.Width.Value, obj.Height.Value);
            }

            if (obj.Top != null) {
                var top = Math.Max(0, Math.Min(obj.Top.Value, Screen.FromControl(_form).Bounds.Height - 100));
                _form.Top = top;
            }

            if (obj.Left != null) {
                var left = Math.Max(0, Math.Min(obj.Left.Value, Screen.FromControl(_form).Bounds.Width - 100));
                _form.Left = left;
            }
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e) {
            if (_form != null) {
                var screenInfo = Screen.FromControl(_form);
                var state = _form.WindowState == FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
                var height = Math.Max(_form.Size.Height, 100);
                var width = Math.Max(_form.Size.Width, 100);
                var top = Math.Max(0, Math.Min(_form.Top, screenInfo.Bounds.Height - 100));
                var left = Math.Max(0, Math.Min(_form.Left, screenInfo.Bounds.Width - 100));

                var props = new WindowSizeProps {
                    State = (int)state,
                    Top = top,
                    Left = left,
                    Width = width,
                    Height = height,
                    Display = screenInfo.DeviceName
                };

                _settingsService.GetLocal().WindowPositionSettings = props;
                _form = null;
            }
        }

        public class WindowSizeProps {
            public int State { get; set; }
            public int? Height { get; set; }
            public int? Width { get; set; }
            public int? Top { get; set; }
            public int? Left { get; set; }
            public string Display { get; set; }
        }
        
    }
}

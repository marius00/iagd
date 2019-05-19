using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using Newtonsoft.Json;

namespace IAGrim.UI.Misc {
    /// <summary>
    /// Responsible for persisting the window size and location across runs.
    /// </summary>
    public class WindowSizeManager {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        private Form _form;
        private readonly SettingsService _settingsService;

        public WindowSizeManager(Form form, SettingsService settingsService) {
            _form = form;
            _settingsService = settingsService;
            _form.FormClosing += _form_FormClosing;

            var obj = _settingsService.GetLocal().WindowPositionSettings;
            if (obj?.State != null) {
                _form.WindowState = (FormWindowState)obj.State;
            }
            if (obj?.Width != null && obj?.Height != null) {
                _form.Size = new Size(obj.Width.Value, obj.Height.Value);
            }


            if (obj?.Top != null) {
                var top = Math.Max(0, Math.Min(obj.Top.Value, Screen.FromControl(_form).Bounds.Height - 100));
                _form.Top = top;
            }

            if (obj?.Left!= null) {
                var left = Math.Max(0, Math.Min(obj.Left.Value, Screen.FromControl(_form).Bounds.Width - 100));
                _form.Left = left;
            }
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e) {
            if (_form != null) {
                var state = _form.WindowState == FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
                var height = Math.Max(_form.Size.Height, 100);
                var width = Math.Max(_form.Size.Width, 100);
                var top = Math.Max(0, Math.Min(_form.Top, Screen.FromControl(_form).Bounds.Height - 100));
                var left = Math.Max(0, Math.Min(_form.Left, Screen.FromControl(_form).Bounds.Width - 100));

                var props = new WindowSizeProps {
                    State = (int)state,
                    Top = top,
                    Left = left,
                    Width = width,
                    Height = height
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
        }
        
    }
}

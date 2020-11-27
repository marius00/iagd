using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace IAGrim.UI.Misc {
    class MinimizeToTrayHandler : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MinimizeToTrayHandler));
        private FormWindowState _previousWindowState = FormWindowState.Normal;
        private Form _form;
        private readonly NotifyIcon _notifyIcon;

        public MinimizeToTrayHandler(Form form, NotifyIcon notifyIcon) {
            _form = form;
            _notifyIcon = notifyIcon;
            _form.SizeChanged += OnMinimizeWindow;
            _notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
        }

        public bool MinimizeToTray { get; set; } = true;

        public void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
            _form.Visible = true;
            _notifyIcon.Visible = false;
            _form.WindowState = _previousWindowState;
        }

        /// <summary>
        /// Minimize to tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object sender, EventArgs e) {
            try {
                if (MinimizeToTray) {
                    if (_form.WindowState == FormWindowState.Minimized) {
                        _form.Hide();
                        _notifyIcon.Visible = true;
                    }
                    else {
                        _notifyIcon.Visible = false;
                        _previousWindowState = _form.WindowState;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }
        }

        public void Dispose() {
            var f = _form;
            if (f != null) {
                f.SizeChanged -= OnMinimizeWindow;
            }

            _form = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using IAGrim.Parsers.Arz;
using IAGrim.Utilities;

namespace IAGrim.UI {
    public partial class DonateNagScreen : Form {
        readonly System.Timers.Timer _aTimer = new System.Timers.Timer(50);
        readonly Color _graycolor = Color.FromArgb(240, 240, 240);
        readonly Color _greenish = Color.FromArgb(220, 224, 210);
        const int Timer = 600;
        const int NumSteps = Timer / 50;
        int _nagDelay = 2200;
        
        private int _currentStep = 0;
        private bool _greening = true;

        public DonateNagScreen() {
            InitializeComponent();
            this.FormClosing += DonateNagScreen_FormClosing;
        }

        void DonateNagScreen_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason != CloseReason.WindowsShutDown && _nagDelay > 0)
                e.Cancel = true;
        }

        public static bool CanNag {
            get {
                long lastNag = Properties.Settings.Default.LastNagTimestamp;
                if (lastNag == 0) {
                    DateTime dt = DateTime.Now.AddDays(new Random().Next(5, 14));
                    Properties.Settings.Default.LastNagTimestamp = dt.Ticks;
                    Properties.Settings.Default.Save();
                    return false;
                }

                return lastNag < DateTime.Now.Ticks;
            }
        }

        private void DonateNagScreen_Load(object sender, EventArgs e) {
            if (CanNag) {
                _aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                _aTimer.Interval = 50;
                _aTimer.Enabled = true;

                DateTime dt = DateTime.Now.AddDays(28 + new Random().Next(0, 5));
                Properties.Settings.Default.LastNagTimestamp = dt.Ticks;
                Properties.Settings.Default.Save();

                LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
            }
            else {
                _nagDelay = -1;
                this.Close();
            }

        }

        void OnTimedEvent(object source, ElapsedEventArgs e) {
            int r = (_graycolor.R - _greenish.R) / NumSteps;
            int g = (_graycolor.G - _greenish.G) / NumSteps;
            int b = (_graycolor.B - _greenish.B) / NumSteps;

            r = _greenish.R + r * _currentStep;
            g = _greenish.G + g * _currentStep;
            b = _greenish.B + b * _currentStep;

            panel1.BackColor = Color.FromArgb(r, g, b);

            if (_greening)
                _currentStep++;
            else
                _currentStep--;

            if (_currentStep <= 0) {
                _greening = !_greening;
                _currentStep = 1;
            }
            else if (_currentStep >= NumSteps) {
                _greening = !_greening;
                _currentStep = NumSteps - 1;
            }

            _nagDelay -= 50;

            if (_nagDelay <= 0) {
                buttonNoThanks.Enabled = true;
            }
        }

        private void buttonNoThanks_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://grimdawn.dreamcrash.org/ia/?donate");
            

            DateTime dt = DateTime.Now.AddDays(62 + new Random().Next(0, 5));
            Properties.Settings.Default.LastNagTimestamp = dt.Ticks;
            Properties.Settings.Default.Save();            
            
        }
    }
}

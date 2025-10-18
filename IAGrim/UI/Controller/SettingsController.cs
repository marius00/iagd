using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;

namespace IAGrim.UI.Controller {
    [Obsolete]
    class SettingsController : INotifyPropertyChanged, ISettingsController, ISettingsReadController {
        private readonly SettingsService _settings;


        public SettingsController(SettingsService settings) {
            _settings = settings;
        }

        public void LoadDefaults() {
            MinimizeToTray = _settings.GetPersistent().MinimizeToTray;
            AutoUpdateModSettings = _settings.GetPersistent().AutoUpdateModSettings;
            HideSkills = _settings.GetPersistent().HideSkills;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Automatically update the selected mod when changed ingame
        /// Also goes for softcore/hardcore
        /// </summary>
        public bool AutoUpdateModSettings {
            get => _settings.GetPersistent().AutoUpdateModSettings;
            private set {
                _settings.GetPersistent().AutoUpdateModSettings = value;
                OnPropertyChanged();
            }
        }


        public bool HideSkills {
            get => _settings.GetPersistent().HideSkills;
            set {
                _settings.GetPersistent().HideSkills = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Minimize the program to the system tray
        /// </summary>
        public bool MinimizeToTray {
            get => _settings.GetPersistent().MinimizeToTray;
            set {
                _settings.GetPersistent().MinimizeToTray = value;
                OnPropertyChanged();
            }
        }


        #endregion

        private string StripPrefix(string s) {
            if (s.StartsWith("cb"))
                return s.Substring(2);
            else if (s.StartsWith("radio"))
                return s.Substring(4);
            else if (s.StartsWith("tb"))
                return s.Substring(2);
            else
                return s;
        }

        public void BindCheckbox(FirefoxCheckBox control) {
            string name = StripPrefix(control.Name);
            control.DataBindings.Add(new Binding("Checked", this, name, false, DataSourceUpdateMode.OnPropertyChanged));
        }

        public void BindCheckbox(Control control, string property) {
            control.DataBindings.Add(new Binding("Checked", this, property));
        }

        public void BindText(Control control) {
            control.DataBindings.Add(new Binding("Text", this, StripPrefix(control.Name)));
        }

        public void BindText(Control control, string property) {
            control.DataBindings.Add(new Binding("Text", this, property));
        }

        public void DonateNow() {
            Process.Start(new ProcessStartInfo { FileName = "https://grimdawn.evilsoft.net/?donate", UseShellExecute = true });
            DateTime dt = DateTime.Now.AddDays(new Random().Next(14, 25));
            _settings.GetLocal().LastNagTimestamp = dt.Ticks;
        }

        public void OpenDataFolder() {
            Process.Start(new ProcessStartInfo { FileName = "file://" + GlobalPaths.BackupLocation, UseShellExecute = true });
        }


        public void OpenLogFolder() {
            Process.Start(new ProcessStartInfo { FileName = "file://" + GlobalPaths.CoreFolder, UseShellExecute = true });
        }
    }
}
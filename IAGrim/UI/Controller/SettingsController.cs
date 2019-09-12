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
    class SettingsController : INotifyPropertyChanged, ISettingsController, ISettingsReadController {
        private readonly SettingsService _settings;


        public SettingsController(SettingsService settings) {
            _settings = settings;
        }


        public void LoadDefaults() {
            MinimizeToTray = _settings.GetPersistent().MinimizeToTray;
            MergeDuplicates = _settings.GetPersistent().MergeDuplicates;
            TransferAnyMod = _settings.GetPersistent().TransferAnyMod;
            SecureTransfers = _settings.GetLocal().SecureTransfers ?? true;
            ShowRecipesAsItems = _settings.GetPersistent().ShowRecipesAsItems;
            AutoUpdateModSettings = _settings.GetPersistent().AutoUpdateModSettings;
            DisplaySkills = _settings.GetPersistent().DisplaySkills;
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

        /// <summary>
        /// List recipes along with items
        /// </summary>
        public bool ShowRecipesAsItems {
            get => _settings.GetPersistent().ShowRecipesAsItems;
            set {
                _settings.GetPersistent().ShowRecipesAsItems = value;
                OnPropertyChanged();
            }
        }

        public bool DisplaySkills {
            get => _settings.GetPersistent().DisplaySkills;
            set {
                _settings.GetPersistent().DisplaySkills = value;
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


        /// <summary>
        /// Merge duplicate items into a single entry
        /// </summary>
        public bool MergeDuplicates {
            get => _settings.GetPersistent().MergeDuplicates;
            set {
                _settings.GetPersistent().MergeDuplicates = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Transfer to any mod without restrictions
        /// </summary>
        public bool TransferAnyMod {
            get => _settings.GetPersistent().TransferAnyMod;
            set {
                _settings.GetPersistent().TransferAnyMod = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Enable DLL stash-closed safety checks
        /// </summary>
        public bool SecureTransfers {
            get => _settings.GetLocal().SecureTransfers ?? true;
            set {
                if (value || MessageBox.Show(
                        RuntimeSettings.Language.GetTag("iatag_ui_settings_securetransferdsable_body"),
                        RuntimeSettings.Language.GetTag("iatag_ui_settings_securetransferdsable_title"),
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes) {
                    _settings.GetLocal().SecureTransfers = value;
                    OnPropertyChanged();
                }
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
            System.Diagnostics.Process.Start("http://grimdawn.dreamcrash.org/ia/?donate");
            DateTime dt = DateTime.Now.AddDays(new Random().Next(14, 25));
            _settings.GetLocal().LastNagTimestamp = dt.Ticks;
        }

        public void OpenDataFolder() {
            String appdata = Environment.GetEnvironmentVariable("LocalAppData");
            string dir = Path.Combine(appdata, "EvilSoft", "IAGD", "backup");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Process.Start("file://" + dir);
        }


        public void OpenLogFolder() {
            String appdata = Environment.GetEnvironmentVariable("LocalAppData");
            string dir = Path.Combine(appdata, "EvilSoft", "IAGD");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Process.Start("file://" + dir);
        }
    }
}
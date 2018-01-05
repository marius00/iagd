using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using IAGrim.Utilities.HelperClasses;

namespace IAGrim.UI.Controller {

    class SettingsController : INotifyPropertyChanged, ISettingsController, ISettingsReadController {


        public SettingsController() {
            //radioBeta.Checked = (bool)Properties.Settings.Default["SubscribeExperimentalUpdates"];
            //radioRelease.Checked = !(bool)Properties.Settings.Default["SubscribeExperimentalUpdates"];
        }


        public void LoadDefaults() {
            MinimizeToTray = (bool)Properties.Settings.Default.MinimizeToTray;
            MergeDuplicates = (bool)Properties.Settings.Default.MergeDuplicates;
            TransferAnyMod = (bool)Properties.Settings.Default.TransferAnyMod;
            SecureTransfers = (bool)Properties.Settings.Default.SecureTransfers;
            ShowRecipesAsItems = (bool)Properties.Settings.Default.ShowRecipesAsItems;
            AutoUpdateModSettings = (bool)Properties.Settings.Default.AutoUpdateModSettings;
            InstaTransfer = (bool)Properties.Settings.Default.InstaTransfer;
            AutoSearch = (bool)Properties.Settings.Default.AutoSearch;
            DisplaySkills = Properties.Settings.Default.DisplaySkills;
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
            get {
                return (bool)Properties.Settings.Default.AutoUpdateModSettings;
            }
            set {
                Properties.Settings.Default.AutoUpdateModSettings = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List recipes along with items
        /// </summary>
        public bool ShowRecipesAsItems {
            get {
                return (bool)Properties.Settings.Default.ShowRecipesAsItems;
            }
            set {
                Properties.Settings.Default.ShowRecipesAsItems = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool DisplaySkills {
            get {
                return Properties.Settings.Default.DisplaySkills;
            }
            set {
                Properties.Settings.Default.DisplaySkills = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Minimize the program to the system tray
        /// </summary>
        public bool MinimizeToTray {
            get {
                return (bool)Properties.Settings.Default.MinimizeToTray;
            }
            set {
                if ((bool)Properties.Settings.Default.MinimizeToTray != value) {
                    Properties.Settings.Default.MinimizeToTray = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }
            

        /// <summary>
        /// Merge duplicate items into a single entry
        /// </summary>
        public bool MergeDuplicates {
            get {
                return (bool)Properties.Settings.Default.MergeDuplicates;
            }
            set {
                Properties.Settings.Default.MergeDuplicates = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Transfer to any mod without restrictions
        /// </summary>
        public bool TransferAnyMod {
            get {
                return (bool)Properties.Settings.Default.TransferAnyMod;
            }
            set {
                Properties.Settings.Default.TransferAnyMod = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Automatically update the item view when a filter changes?
        /// </summary>
        public bool AutoSearch {
            get {
                return (bool)Properties.Settings.Default.AutoSearch;
            }
            set {
                Properties.Settings.Default.AutoSearch = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }


        public bool InstaTransfer {
            get {
                return (bool)Properties.Settings.Default.InstaTransfer;
            }
            set {
                Properties.Settings.Default.InstaTransfer = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// Enable DLL stash-closed safety checks
        /// </summary>
        public bool SecureTransfers {
            get {
                return (bool)Properties.Settings.Default.SecureTransfers;
            }
            set {
                if (value || MessageBox.Show("Are you sure you wish to disable secure transfers?\n\nIt will be YOUR responsibility to make sure the bank is closed when transferring.", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Properties.Settings.Default.SecureTransfers = value;
                    Properties.Settings.Default.Save();
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
            DateTime dt = DateTime.Now.AddDays(new Random().Next(14,25));
            Properties.Settings.Default.LastNagTimestamp = dt.Ticks;
            Properties.Settings.Default.Save();
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

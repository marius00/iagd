using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    public class PersistentSettings {
        public event EventHandler OnMutate;

        private long? _buddySyncUserIdV3;

        // Settings
        private bool _subscribeExperimentalUpdates;
        private bool _showRecipesAsItems;
        private bool _minimizeToTray;
        private bool _usingDualComputer;
        private bool _showAugmentsAsItems;
        private bool _mergeDuplicates;
        private bool _transferAnyMod;
        private bool _darkMode;
        private bool _autoUpdateModSettings;
        private bool _hideSkills;
        private bool _autoDismissNotifications;

        private long _cloudUploadTimestamp;


        // Azure Backups
        [Obsolete]
        private string _azureAuthToken;
        
        private string _cloudAuthToken;
        private string _cloudUser;

        // Buddy items
        public long? BuddySyncUserIdV3 {
            get => _buddySyncUserIdV3;
            set {
                _buddySyncUserIdV3 = value;
                OnMutate?.Invoke(null, null);
            }
        }

        // Settings
        public bool SubscribeExperimentalUpdates {
            get => _subscribeExperimentalUpdates;
            set {
                _subscribeExperimentalUpdates = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool ShowRecipesAsItems {
            get => _showRecipesAsItems;
            set {
                _showRecipesAsItems = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool MinimizeToTray {
            get => _minimizeToTray;
            set {
                _minimizeToTray = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool UsingDualComputer {
            get => _usingDualComputer;
            set {
                _usingDualComputer = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool ShowAugmentsAsItems {
            get => _showAugmentsAsItems;
            set {
                _showAugmentsAsItems = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool MergeDuplicates {
            get => _mergeDuplicates;
            set {
                _mergeDuplicates = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool TransferAnyMod {
            get => _transferAnyMod;
            set {
                _transferAnyMod = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool DarkMode {
            get => _darkMode;
            set {
                _darkMode = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool AutoUpdateModSettings {
            get => _autoUpdateModSettings;
            set {
                _autoUpdateModSettings = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string UUID { get; set; }

        public bool HideSkills {
            get => _hideSkills;
            set {
                _hideSkills = value;
                OnMutate?.Invoke(null, null);
            }
        }

        // Azure Backups
        [Obsolete]
        public string AzureAuthToken {
            get => _azureAuthToken;
            set {
                _azureAuthToken = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string CloudAuthToken {
            get => _cloudAuthToken;
            set {
                _cloudAuthToken = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string CloudUser {
            get => _cloudUser;
            set {
                _cloudUser = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool AutoDismissNotifications {
            get => _autoDismissNotifications;
            set {
                _autoDismissNotifications = value;
                OnMutate?.Invoke(null, null);
            }
        }


        public long CloudUploadTimestamp {
            get => _cloudUploadTimestamp;
            set {
                _cloudUploadTimestamp = value;
                OnMutate?.Invoke(null, null);
            }
        }
        
        
    }
}
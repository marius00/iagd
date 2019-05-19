using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    public class PersistentSettings {
        public event EventHandler OnMutate;

        private long? _buddySyncUserIdV2;
        private string _buddySyncDescription;
        private bool _buddySyncEnabled;

        // Settings
        private bool _subscribeExperimentalUpdates;
        private bool _showRecipesAsItems;
        private bool _minimizeToTray;
        private bool _usingDualComputer;
        private bool _showAugmentsAsItems;
        private bool _mergeDuplicates;
        private bool _transferAnyMod;
        private bool _autoUpdateModSettings;
        private bool _displaySkills;
        private bool _deleteDuplicates;

        // Azure Backups
        private string _azureAuthToken;


        // Buddy items
        public long? BuddySyncUserIdV2 {
            get => _buddySyncUserIdV2;
            set {
                _buddySyncUserIdV2 = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string BuddySyncDescription {
            get => _buddySyncDescription;
            set {
                _buddySyncDescription = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool BuddySyncEnabled {
            get => _buddySyncEnabled;
            set {
                _buddySyncEnabled = value;
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

        public bool AutoUpdateModSettings {
            get => _autoUpdateModSettings;
            set {
                _autoUpdateModSettings = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool DisplaySkills {
            get => _displaySkills;
            set {
                _displaySkills = value;
                OnMutate?.Invoke(null, null);
            }
        }

        // Azure Backups
        public string AzureAuthToken {
            get => _azureAuthToken;
            set {
                _azureAuthToken = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool DeleteDuplicates {
            get => _deleteDuplicates;
            set {
                _deleteDuplicates = value;
                OnMutate?.Invoke(null, null);
            }
        }

        
    }
}
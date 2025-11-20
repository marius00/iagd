namespace IAGrim.Settings.Dto {
    public class PersistentSettings {
        public event EventHandler? OnMutate;

        private long? _buddySyncUserIdV3;

        // Settings
        private bool? _subscribeExperimentalUpdates;
        private DateTime? _nextUpdateCheck;
        private bool? _minimizeToTray;
        private bool? _usingDualComputer;
        private bool? _transferAnyMod;
        private bool? _darkMode;
        private bool? _autoUpdateModSettings;
        private bool? _hideSkills;
        private bool? _autoDismissNotifications;

        private long? _cloudUploadTimestamp;

        
        private string? _cloudAuthToken;
        private string? _cloudUser;

        // Buddy items
        public long? BuddySyncUserIdV3 {
            get => _buddySyncUserIdV3;
            set {
                _buddySyncUserIdV3 = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        // Settings
        public bool SubscribeExperimentalUpdates {
            get => _subscribeExperimentalUpdates ?? false;
            set {
                _subscribeExperimentalUpdates = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }
        public DateTime NextUpdateCheck {
            get => _nextUpdateCheck ?? DateTime.UtcNow.AddDays(5);
            set {
                _nextUpdateCheck = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool MinimizeToTray {
            get => _minimizeToTray ?? true;
            set {
                _minimizeToTray = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool UsingDualComputer {
            get => _usingDualComputer ?? false;
            set {
                _usingDualComputer = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool TransferAnyMod {
            get => _transferAnyMod ?? false;
            set {
                _transferAnyMod = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool DarkMode {
            get => _darkMode ?? false;
            set {
                _darkMode = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool AutoUpdateModSettings {
            get => _autoUpdateModSettings ?? true;
            set {
                _autoUpdateModSettings = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public string UUID { get; set; }

        public bool HideSkills {
            get => _hideSkills ?? true;
            set {
                _hideSkills = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public string? CloudAuthToken {
            get => _cloudAuthToken;
            set {
                _cloudAuthToken = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public string? CloudUser {
            get => _cloudUser;
            set {
                _cloudUser = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }

        public bool AutoDismissNotifications {
            get => _autoDismissNotifications ?? true;
            set {
                _autoDismissNotifications = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }


        public long CloudUploadTimestamp {
            get => _cloudUploadTimestamp ?? 0;
            set {
                _cloudUploadTimestamp = value;
                OnMutate?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
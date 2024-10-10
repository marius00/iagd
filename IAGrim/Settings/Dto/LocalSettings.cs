using System;
using System.Collections.Generic;
using IAGrim.UI.Misc;
using IAGrim.Utilities.HelperClasses;
using Newtonsoft.Json;

namespace IAGrim.Settings.Dto {
    public class LocalSettings {
        public event EventHandler OnMutate;

        private List<string> _grimDawnLocation;
        private string _currentGrimdawnLocation;
        private int _backupNumber;
        private long _lastNagTimestamp;
        private bool _easterPrank;
        private bool _hasSuggestedLanguageChange;
        private int _stashToDepositTo;
        private bool _pendingInstantSyncWarning;
        private int _stashToLootFrom;
        private GDTransferFile _lastSelectedMod;
        private string _lastSelectedTargetMod;
        private bool _lastSelectedTargetModIsHc;
        private string _localizationFile;
        private WindowSizeManager.WindowSizeProps _windowPositionSettings;
        private bool _backupDropbox;
        private bool _backupGoogle;
        private bool _backupOnedrive;
        private bool _backupCustom;
        private bool _optOutOfBackups;
        private string _backupCustomLocation;
        private bool _hasWarnedGrimDawnUpdate;
        private bool _isGrimDawnParsed;
        private long? _grimDawnLocationLastModified;
        private bool _startMinimized;
        private DateTime _lastCharSyncUtc;
        private bool _preferLegacyMode;

        public string MachineName { get; set; }

        public void AddGrimDawnLocation(string location) {
            if (_grimDawnLocation == null) {
                _grimDawnLocation = new List<string>(1);
            }

            if (!_grimDawnLocation.Contains(location)) {
                _grimDawnLocation.Add(location);
            }

            OnMutate?.Invoke(null, null);
        }

        public List<string> GrimDawnLocation {
            get => _grimDawnLocation;
            set {
                _grimDawnLocation = value;
                OnMutate?.Invoke(null, null);
            }
        }


        public int BackupNumber {
            get => _backupNumber;
            set {
                _backupNumber = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public long LastNagTimestamp {
            get => _lastNagTimestamp;
            set {
                _lastNagTimestamp = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool EasterPrank {
            get => _easterPrank;
            set {
                _easterPrank = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool StartMinimized {
            get => _startMinimized;
            set {
                _startMinimized = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool HasSuggestedLanguageChange {
            get => _hasSuggestedLanguageChange;
            set {
                _hasSuggestedLanguageChange = value;
                OnMutate?.Invoke(null, null);
            }
        }


        public int StashToDepositTo {
            get => _stashToDepositTo;
            set {
                _stashToDepositTo = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool PendingInstantSyncWarning {
            get => _pendingInstantSyncWarning;
            set {
                _pendingInstantSyncWarning = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public int StashToLootFrom {
            get => _stashToLootFrom;
            set {
                _stashToLootFrom = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public GDTransferFile LastSelectedMod {
            get => _lastSelectedMod;
            set {
                _lastSelectedMod = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string LastSelectedTargetMod {
            get => _lastSelectedTargetMod;
            set {
                _lastSelectedTargetMod = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool LastSelectedTargetModIsHc{
            get => _lastSelectedTargetModIsHc;
            set {
                _lastSelectedTargetModIsHc = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string LocalizationFile {
            get => _localizationFile;
            set {
                _localizationFile = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string CurrentGrimdawnLocation {
            get => _currentGrimdawnLocation;
            set {
                _currentGrimdawnLocation = value;
                OnMutate?.Invoke(null, null);
            }
        }
        

        public WindowSizeManager.WindowSizeProps WindowPositionSettings {
            get => _windowPositionSettings;
            set {
                _windowPositionSettings = value;
                OnMutate?.Invoke(null, null);
            }
        }

        // Backup
        public bool BackupDropbox {
            get => _backupDropbox;
            set {
                _backupDropbox = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool BackupGoogle {
            get => _backupGoogle;
            set {
                _backupGoogle = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool BackupOnedrive {
            get => _backupOnedrive;
            set {
                _backupOnedrive = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool BackupCustom {
            get => _backupCustom;
            set {
                _backupCustom = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public string BackupCustomLocation {
            get => _backupCustomLocation;
            set {
                _backupCustomLocation = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool OptOutOfBackups {
            get => _optOutOfBackups;
            set {
                _optOutOfBackups = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public long GrimDawnLocationLastModified {
            get => _grimDawnLocationLastModified ?? 0;
            set {
                _grimDawnLocationLastModified = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool HasWarnedGrimDawnUpdate {
            get => _hasWarnedGrimDawnUpdate;
            set {
                _hasWarnedGrimDawnUpdate = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool IsGrimDawnParsed {
            get => _isGrimDawnParsed;
            set {
                _isGrimDawnParsed = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public DateTime LastCharSyncUtc {
            get => _lastCharSyncUtc;
            set {
                _lastCharSyncUtc = value;
                OnMutate?.Invoke(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PreferLegacyMode {
            get => _preferLegacyMode;
            set {
                _preferLegacyMode = value;
                OnMutate?.Invoke(null, null);
            }
        }

    }
}
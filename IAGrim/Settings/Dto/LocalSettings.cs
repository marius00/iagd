using System;
using System.Collections.Generic;
using IAGrim.UI.Misc;
using IAGrim.Utilities.HelperClasses;
using Newtonsoft.Json;

namespace IAGrim.Settings.Dto {
    public class LocalSettings {
        public event EventHandler OnMutate;

        private List<string> _grimDawnLocation;
        private int _backupNumber;
        private long _lastNagTimestamp;
        private bool _easterPrank;
        private bool _hasSuggestedLanguageChange;
        private bool? _secureTransfers;
        private int _stashToDepositTo;
        private int _stashToLootFrom;
        private GDTransferFile _lastSelectedMod;
        private string _localizationFile;
        private WindowSizeManager.WindowSizeProps _windowPositionSettings;
        private bool _backupDropbox;
        private bool _backupGoogle;
        private bool _backupOnedrive;
        private bool _backupCustom;
        private bool _optOutOfBackups;
        private string _backupCustomLocation;

        public string MachineName { get; set; }

        public List<string> GrimDawnLocation {
            get => _grimDawnLocation;
            set {
                _grimDawnLocation = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public long? _grimDawnLocationLastModified;

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

        public bool HasSuggestedLanguageChange {
            get => _hasSuggestedLanguageChange;
            set {
                _hasSuggestedLanguageChange = value;
                OnMutate?.Invoke(null, null);
            }
        }

        public bool? SecureTransfers {
            get => _secureTransfers;
            set {
                _secureTransfers = value;
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

        public string LocalizationFile {
            get => _localizationFile;
            set {
                _localizationFile = value;
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
    }
}

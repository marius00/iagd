using System;
using System.Collections.Generic;
using System.IO;
using IAGrim.Settings.Dto;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAGrim.Settings {
    public class SettingsService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsService));

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
        };

        private readonly SettingsTemplate _data;
        private readonly string _persistentStorage;
        private int _numSequentialErrors;

        // TODO: Wipe out local settings if the PC has changed.
        private SettingsService(SettingsTemplate data, string persistentStorage) {
            _data = data;

            var local = _data.Local;
            if (local?.MachineName != Environment.MachineName) {
                Logger.Warn($"Local settings are for {local?.MachineName}, expected {Environment.MachineName}. Discarding local settings.");
                local = new LocalSettings {
                    MachineName = Environment.MachineName
                };
                _data.Local = local;
            }

            var persistent = _data.Persistent;
            if (persistent == null) {
                persistent = new PersistentSettings();
                _data.Persistent = persistent;
            }

            _persistentStorage = persistentStorage;
            local.OnMutate += (sender, args) => Persist();
            persistent.OnMutate += (sender, args) => Persist();
        }

        public LocalSettings GetLocal() {
            // Invariant established in the constructor: _data.Local is always assigned a non-null value.
            return _data.Local!;
        }

        public PersistentSettings GetPersistent() {
            // Invariant established in the constructor: _data.Persistent is always assigned a non-null value.
            return _data.Persistent!;
        }


        private void Persist() {
            string json = JsonConvert.SerializeObject(_data, Formatting.Indented, Settings);
            try {
                File.WriteAllText(_persistentStorage, json);
                _numSequentialErrors = 0;
            }
            catch (Exception ex) {
                if (_numSequentialErrors++ > 5) {
                    Logger.Warn("Reached max consecutive errors attempting to store settings");
                    throw;
                }
                else {
                    Logger.Warn("Error storing settings, once of twice of these is unfortunate but can live with it..", ex);
                }
            }
        }

        public static SettingsService Load(string filename) {
            if (File.Exists(filename)) {
                try {
                    string json = File.ReadAllText(filename);
                    var template = JsonConvert.DeserializeObject<SettingsTemplate>(json, Settings);
                    if (template != null) {
                        return new SettingsService(template, filename);
                    }
                }
                catch (IOException ex) {
                    Logger.Error($"Error reading settings from {filename}, discarding settings.", ex);
                }
                catch (JsonReaderException ex) {
                    Logger.Error($"Error parsing settings from {filename}, discarding settings.", ex);
                }
            }

            Logger.Info("Could not find settings JSON, defaulting to no settings.");
            return new SettingsService(new SettingsTemplate {
                Local = new LocalSettings { MachineName = Environment.MachineName },
                Persistent = new PersistentSettings {
                    AutoDismissNotifications = true,
                }
            }, filename);
        }
    }
}
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
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly SettingsTemplate _data;
        private readonly string _persistentStorage;
        private int _numSequentialErrors;

        // TODO: Wipe out local settings if the PC has changed.
        private SettingsService(SettingsTemplate data, string persistentStorage) {
            _data = data;
            _persistentStorage = persistentStorage;
            _data.Local.OnMutate += (sender, args) => Persist();
            _data.Persistent.OnMutate += (sender, args) => Persist();
        }

        public LocalSettings GetLocal() {
            return _data.Local;
        }

        public PersistentSettings GetPersistent() {
            return _data.Persistent;
        }
        

        private void Persist() {
            string json = JsonConvert.SerializeObject(_data, Settings);
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
                string json = File.ReadAllText(filename);
                return new SettingsService(JsonConvert.DeserializeObject<SettingsTemplate>(json, Settings), filename);
            }
            else {
                Logger.Info("Could not find settings JSON, defaulting to no settings.");
                return new SettingsService(new SettingsTemplate {
                    Local = new LocalSettings(),
                    Persistent = new PersistentSettings()
                }, filename);
            }
        }
    }
}

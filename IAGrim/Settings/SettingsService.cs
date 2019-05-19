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
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        private readonly SettingsTemplate _data;
        private readonly string _persistentStorage;
        private int _numSequentialErrors;

        // TODO: Wipe out local settings if the PC has changed.
        private SettingsService(SettingsTemplate data, string persistentStorage) {
            _data = data;
            _persistentStorage = persistentStorage;
        }

        public bool GetBool(PersistentSetting setting) {
            var value = GetPersistent(setting);
            if (value == null) {
                return false;
            }

            return (bool)value;
        }

        public bool? GetBoolOrNull(PersistentSetting setting) {
            var value = GetPersistent(setting);
            return (bool?)value;
        }

        public string GetString(PersistentSetting setting) {
            var value = GetPersistent(setting);
            return value?.ToString() ?? string.Empty;
        }

        public long GetLong(PersistentSetting setting) {
            var value = GetPersistent(setting);

            if (value is long l) {
                return l;
            }

            if (value is int i) {
                return i;
            }

            return 0L;
        }

        public bool GetBool(LocalSetting setting) {
            var value = GetLocal(setting);
            if (value == null) {
                return false;
            }

            return (bool)value;
        }

        public bool? GetBoolOrNull(LocalSetting setting) {
            var value = GetLocal(setting);
            return (bool?)value;
        }

        public string GetString(LocalSetting setting) {
            var value = GetLocal(setting);
            return value?.ToString() ?? string.Empty;
        }

        public long GetLong(LocalSetting setting) {
            var value = GetLocal(setting);

            if (value != null && value is long) {
                return (long)value;
            }

            if (value != null && value is int) {
                return (int)value;
            }

            return 0L;
        }

        public List<string> GetStrings(LocalSetting setting) {
            var value = GetLocal(setting);
            if (value is List<string> list) {
                return list;
            } else if (value is JArray arr) {
                return arr?.ToObject<List<string>>() ?? new List<string>();
            }
            else {
                return new List<string>();
            }
        }

        public T Get<T>(LocalSetting setting) {
            //var obj = GetLocal(setting);
            //return JsonConvert.DeserializeObject<T>(obj);
            return default(T);  // TODO:
        }

        private object GetLocal(LocalSetting setting) {
            var key = setting.ToString().ToLowerInvariant();
            if (_data.Local.ContainsKey(key)) {
                return _data.Local[key];
            }

            return null;
        }

        private object GetPersistent(PersistentSetting setting) {
            var key = setting.ToString().ToLowerInvariant();
            if (_data.Persistent.ContainsKey(key)) {
                return _data.Persistent[key];
            }

            return null;
        }

        public void Save(LocalSetting setting, object value) {
            var key = setting.ToString().ToLowerInvariant();
            _data.Local[key] = value;

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

        public void Save(PersistentSetting setting, object value) {
            var key = setting.ToString().ToLowerInvariant();
            _data.Persistent[key] = value;

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
                Logger.Info("Could not find settings JSON, defaulting to no settings."); // TODO: Migrate from old format!
                return new SettingsService(new SettingsTemplate {
                    Local = new Dictionary<string, object>(),
                    Persistent = new Dictionary<string, object>()
                }, filename);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Properties;
using IAGrim.Settings;
using IAGrim.Settings.Dto;

namespace IAGrim.Backup.Azure.Service {
    public class AuthenticationProvider {
        private readonly SettingsService _settings;

        public AuthenticationProvider(SettingsService settings) {
            _settings = settings;
        }

        public string GetToken() {
            return _settings.GetPersistent().AzureAuthToken;
        }

        public bool HasToken() {
            return !string.IsNullOrWhiteSpace(_settings.GetPersistent().AzureAuthToken);
        }

        public void SetToken(string token) {
            _settings.GetPersistent().AzureAuthToken = token;
        } 
    }
}

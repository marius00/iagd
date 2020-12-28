using IAGrim.Settings;

namespace IAGrim.Backup.Cloud.Service {
    public class AuthenticationProvider {
        private readonly SettingsService _settings;

        public AuthenticationProvider(SettingsService settings) {
            _settings = settings;
        }

        public bool CanMigrate() {
            var cloudToken = _settings.GetPersistent().CloudAuthToken;
            var legacyToken = _settings.GetPersistent().AzureAuthToken;
            return string.IsNullOrEmpty(cloudToken) && !string.IsNullOrEmpty(legacyToken);
        }

        public string GetLegacyToken() {
            return _settings.GetPersistent().AzureAuthToken;
        }

        public string GetToken() {
            return _settings.GetPersistent().CloudAuthToken;
        }

        public string GetUser() {
            return _settings.GetPersistent().CloudUser;
        }

        public bool HasToken() {
            return !string.IsNullOrWhiteSpace(_settings.GetPersistent().CloudAuthToken);
        }

        public void SetToken(string user, string token) {
            _settings.GetPersistent().CloudUser = user;
            _settings.GetPersistent().CloudAuthToken = token;

            // Clear the old access token
            _settings.GetPersistent().AzureAuthToken = null;
        } 
    }
}

using IAGrim.Settings;

namespace IAGrim.Backup.Cloud.Service {
    public class AuthenticationProvider {
        private readonly SettingsService _settings;

        public AuthenticationProvider(SettingsService settings) {
            _settings = settings;
        }

        public string GetToken() {
            return _settings.GetPersistent().CloudAuthToken ?? string.Empty;
        }

        public string GetUser() {
            return _settings.GetPersistent().CloudUser ?? string.Empty;
        }

        public bool HasToken() {
            return !string.IsNullOrWhiteSpace(_settings.GetPersistent().CloudAuthToken);
        }

        public void SetToken(string user, string token) {
            _settings.GetPersistent().CloudUser = user;
            _settings.GetPersistent().CloudAuthToken = token;
        } 
    }
}

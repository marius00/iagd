using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Properties;

namespace IAGrim.Backup.Azure.Service {
    public class AuthenticationProvider {
        public string GetToken() {
            return Settings.Default.AzureAuthToken;
        }

        public bool HasToken() {
            return !string.IsNullOrWhiteSpace(Settings.Default.AzureAuthToken);
        }

        public void SetToken(string token) {
            Settings.Default.AzureAuthToken = token;
            Settings.Default.Save();
        } 
    }
}

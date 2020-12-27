using System;

namespace IAGrim.Backup.Cloud {
    static class Uris {
        public const string EnvLocalDev = "localdev";
        public const string EnvCloud = "cloud";


        public static void Initialize(string env) {
            string host;

            if (env == EnvLocalDev) {
                host = "http://localhost:8080";
            }
            else if (env == EnvCloud) {
                host = "https://api.iagd.evilsoft.net";
            }
            else {
                throw new ArgumentException(env);
            }

            TokenVerificationUri = $"{host}/logincheck";
            UploadItemsUrl = $"{host}/upload";
            DownloadUrl = $"{host}/download";
            DeleteItemsUrl = $"{host}/remove";
            FetchLimitationsUrl = $"{host}/logincheck";
            DeleteAccountUrl = $"{host}/delete";
            LogoutUrl = $"{host}/logout";
            MigrateUrl = $"{host}/migrate";

            LoginPageUrl = "http://iagd.evilsoft.net/";
        }

        public static string LoginPageUrl { get; private set; }
        public static string DownloadUrl { get; private set; }
        public static string TokenVerificationUri { get; private set; }
        public static string UploadItemsUrl { get; private set; }
        public static string DeleteItemsUrl { get; private set; }
        public static string FetchLimitationsUrl { get; private set; }
        public static string DeleteAccountUrl { get; private set; }
        public static string LogoutUrl { get; private set; }
        public static string MigrateUrl { get; private set; }
    }
}
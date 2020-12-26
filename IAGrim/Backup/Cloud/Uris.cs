using System;

namespace IAGrim.Backup.Cloud {
    static class Uris {
        public const string EnvLocalDev = "localdev";
        public const string EnvCloud = "cloud";
        private const string CloudURi = "https://api.iagd.evilsoft.net";


        public static void Initialize(string env) {
            if (env == EnvLocalDev) {
                string local = "http://localhost:8080";
                TokenVerificationUri = $"{local}/logincheck";
                UploadItemsUrl = $"{local}/upload";
                DownloadUrl = $"{local}/download";
                DeleteItemsUrl = $"{local}/remove";
                FetchLimitationsUrl = $"{local}/logincheck";
                DeleteAccountUrl = $"{local}/delete";
            }
            else if (env == EnvCloud) {
                LoginPageUrl = "http://iagd.evilsoft.net/";
                TokenVerificationUri = $"{CloudURi}/logincheck";
                UploadItemsUrl = $"{CloudURi}/upload";
                DownloadUrl = $"{CloudURi}/download";
                DeleteItemsUrl = $"{CloudURi}/remove";
                FetchLimitationsUrl = $"{CloudURi}/logincheck";
                DeleteAccountUrl = $"{CloudURi}/delete";
            }
            else {
                throw new ArgumentException(env);
            }
        }

        public static string LoginPageUrl { get; private set; }
        public static string DownloadUrl { get; private set; }
        public static string TokenVerificationUri { get; private set; }
        public static string UploadItemsUrl { get; private set; }
        public static string DeleteItemsUrl { get; private set; }
        public static string FetchLimitationsUrl { get; private set; }
        public static string DeleteAccountUrl { get; private set; }
    }
}

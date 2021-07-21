using System;

namespace IAGrim.Backup.Cloud {
    static class Uris {
        public const string EnvLocalDev = "localdev";
        public const string EnvCloud = "cloud";


        public static void Initialize(string env) {
            string host;

            if (env == EnvLocalDev) {
                host = "https://api.iagd.evilsoft.net";
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
            BuddyItemsUrl = $"{host}/buddyitems";
            GetBuddyIdUrl = $"{host}/buddyId";
            UploadCharacterUrl = $"{host}/character/upload";
            ListCharacterUrl = $"{host}/character";
            DownloadCharacterUrl = $"{host}/character/download";

            LoginPageUrl = "http://iagd.evilsoft.net/login";
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
        public static string BuddyItemsUrl { get; private set; }
        public static string GetBuddyIdUrl { get; private set; } // TODO: This needs to be set by the backup service
        public static string UploadCharacterUrl { get; private set; }
        public static string ListCharacterUrl { get; private set; }
        public static string DownloadCharacterUrl { get; private set; }
    }
}
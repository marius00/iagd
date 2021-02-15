using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Settings;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using IAGrim.Utilities.HelperClasses;
using log4net;

namespace IAGrim.Backup.Cloud.Service {
    class CharacterBackupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CharacterBackupService));
        private readonly SettingsService _settings;
        private readonly AuthService _authService;
        private readonly ActionCooldown _cooldown = new ActionCooldown(1000 * 60);
        private bool _isActive = true;

        public CharacterBackupService(SettingsService settings, AuthService authService) {
            _settings = settings;
            _authService = authService;
        }

        public void SetIsActive(bool b) {
            _isActive = b;
        }

        public void Execute() {
            if (_authService.CheckAuthentication() != AuthService.AccessStatus.Authorized) {
                return;
            }

            if (!_isActive)
                return;

            _cooldown.ExecuteIfReady(ExecuteInternal);
        }


        public List<CharacterListDto> ListBackedUpCharacters() {
            CharacterListDto[] characters = _authService.GetRestService()?.Get<CharacterListDto[]>(Uris.ListCharacterUrl);
            return characters.ToList();
        }

        class CharacterDownloadUrlDto {
            public string Url { get; set; }
        }

        public string GetDownloadUrl(string character) {
            var url = $"{Uris.DownloadCharacterUrl}?name={WebUtility.UrlEncode(character)}";
            return _authService.GetRestService()?.Get<CharacterDownloadUrlDto>(url)?.Url;
        }

        private void ExecuteInternal() {
            var lastSync = _settings.GetLocal().LastCharSyncUtc;
            var highestTimestamp = FileBackup.GetHighestCharacterTimestamp();
            var characters = FileBackup.ListCharactersNewerThan(lastSync);

            foreach (var c in characters) {
                Logger.Info($"Backup up character {c} to the cloud");
                var filename = Path.Combine(GlobalPaths.CharacterBackupLocation, $"{DateTime.Now.DayOfWeek}-{c}.zip");
                FileBackup.BackupCharacter(filename, c); // TODO: IOException
                
                var url = $"{Uris.UploadCharacterUrl}?name={WebUtility.UrlEncode(c)}";
                var success = Post(url, filename);
                if (success) {
                    Logger.Info($"Character {c} successfully backed up to the cloud");
                }
                else {
                    Logger.Info($"An error occurred backing up character {c} to the cloud");
                }
            }

            {
                var filename = Path.Combine(GlobalPaths.CharacterBackupLocation, $"{DateTime.Now.DayOfWeek}-common.zip");
                FileBackup.BackupCommon(filename);
                var url = $"{Uris.UploadCharacterUrl}?name=StashFiles";
                var success = Post(url, filename);
                if (success) {
                    Logger.Info($"Stash files successfully backed up to the cloud");
                }
                else {
                    Logger.Info($"An error occurred backing up stash files to the cloud");
                }
            }

            _settings.GetLocal().LastCharSyncUtc = highestTimestamp;
        }


        private bool Post(string url, string filename) {
            var authProvider = _authService.GetAuthProvider();
            if (authProvider == null) {
                return false;
            }

            try {
                using (var client = new WebClient()) {
                    client.Headers.Add("Authorization", authProvider.GetToken());
                    client.Headers.Add("X-Api-User", authProvider.GetUser());
                    byte[] result = client.UploadFile(url, "POST", filename);
                    var json = Encoding.Default.GetString(result);
                    Logger.Debug($"Upload succeeded");
                    return true;
                }
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
        }

    }
}

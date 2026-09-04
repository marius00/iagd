using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Backup.Cloud.Service {
    class CharacterBackupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CharacterBackupService));

        /// <summary>
        /// Key used for the shared stash files, which are backed up alongside characters but do not belong to any one of them. Contains a separator, so it cannot collide with a real character name.
        /// </summary>
        private const string StashKey = "/stash";

        /// <summary>
        /// The server throttles to one backup per character per day.
        /// </summary>
        private static readonly TimeSpan MinUploadInterval = TimeSpan.FromHours(24);

        private readonly SettingsService _settings;
        private readonly AuthService _authService;
        private readonly ActionCooldown _cooldown = new ActionCooldown(1000 * 60 * 10);
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

            if (!FileBackup.MyDocumentsGrimDawnExists()) {
                return;
            }

            if (!_isActive)
                return;

            _cooldown.ExecuteIfReady(ExecuteInternal);
        }


        public List<CharacterListDto> ListBackedUpCharacters() {
            CharacterListDto[]? characters = _authService.GetRestService()?.Get<CharacterListDto[]>(Uris.ListCharacterUrl!);
            return characters?.ToList() ?? new List<CharacterListDto>(0);
        }

        class CharacterDownloadUrlDto {
            public string? Url { get; set; }
        }

        public string? GetDownloadUrl(string character) {
            var url = $"{Uris.DownloadCharacterUrl}?name={WebUtility.UrlEncode(character)}";
            return _authService.GetRestService()?.Get<CharacterDownloadUrlDto>(url)?.Url;
        }

        private void ExecuteInternal() {
            var backups = _settings.GetLocal().CharacterBackups;
            var mutated = false;

            foreach (var character in FileBackup.ListCharacters()) {
                mutated |= Backup(
                    backups,
                    key: character,
                    remoteName: character,
                    computeHash: () => FileBackup.ComputeCharacterHash(character),
                    writeArchive: filename => FileBackup.BackupCharacter(filename, character),
                    archiveName: $"{DateTime.Now.DayOfWeek}-{character}.zip",
                    description: $"character {character}"
                );
            }

            if (FileBackup.StashFilesExist()) {
                mutated |= Backup(
                    backups,
                    key: StashKey,
                    remoteName: $"StashFiles-{DateTime.Now.DayOfWeek}",
                    computeHash: FileBackup.ComputeStashHash,
                    writeArchive: FileBackup.BackupCommon,
                    archiveName: $"{DateTime.Now.DayOfWeek}-common.zip",
                    description: "stash files"
                );
            }

            if (mutated) {
                _settings.GetLocal().CharacterBackupsChanged();
            }
        }

        /// <summary>
        /// Backs up one character (or the stash) unless the remote already holds it.
        /// </summary>
        /// <returns>Whether the recorded backup state changed.</returns>
        private bool Backup(
            Dictionary<string, CharacterBackupState> backups,
            string key,
            string remoteName,
            Func<string> computeHash,
            Action<string> writeArchive,
            string archiveName,
            string description
        ) {
            try {
                backups.TryGetValue(key, out var known);

                var hash = computeHash();
                if (known != null && known.Hash == hash) {
                    return false;
                }

                // Changed, but the server would reject it as a duplicate for today.
                // Left unrecorded so it is offered again once the window has passed.
                if (known != null && DateTime.UtcNow - known.UploadedUtc < MinUploadInterval) {
                    Logger.Debug($"Skipping {description}, already backed up within the last 24 hours");
                    return false;
                }

                Logger.Info($"Backing up {description} to the cloud");
                var filename = Path.Combine(GlobalPaths.CharacterBackupLocation, archiveName);
                writeArchive(filename);

                var url = $"{Uris.UploadCharacterUrl}?name={WebUtility.UrlEncode(remoteName)}";
                var status = Post(url, filename);

                switch (status) {
                    case UploadStatus.Stored:
                    case UploadStatus.Unchanged:
                        Logger.Info($"Successfully backed up {description} to the cloud");
                        backups[key] = new CharacterBackupState { Hash = hash, UploadedUtc = DateTime.UtcNow };
                        return true;

                    case UploadStatus.Throttled:
                        // The server already holds a backup for today. Record when, but not
                        // the hash, so this is retried after the window rather than dropped.
                        Logger.Info($"Server already holds a backup of {description} for today");
                        backups[key] = new CharacterBackupState { Hash = known?.Hash ?? string.Empty, UploadedUtc = DateTime.UtcNow };
                        return true;

                    default:
                        Logger.Info($"An error occurred backing up {description} to the cloud");
                        return false;
                }
            }
            catch (IOException ex) {
                // One unreadable or locked save must not stop the remaining characters.
                Logger.Warn($"Error creating a backup archive for {description}", ex);
                return false;
            }
            catch (UnauthorizedAccessException ex) {
                Logger.Warn($"Error creating a backup archive for {description}", ex);
                return false;
            }
        }

        private enum UploadStatus {
            Failed,
            Stored,
            Unchanged,
            Throttled,
        }

        private class UploadResponse {
            public string? Status { get; set; }
        }

        private UploadStatus Post(string url, string filename) {
            var authProvider = _authService.GetAuthProvider();
            if (authProvider == null) {
                return UploadStatus.Failed;
            }

            try {
                using (var client = new WebClient()) {
                    client.Headers.Add("Authorization", authProvider.GetToken());
                    client.Headers.Add("X-Api-User", authProvider.GetUser());
                    byte[] result = client.UploadFile(url, "POST", filename);
                    var json = Encoding.UTF8.GetString(result);
                    Logger.Debug($"Upload succeeded");

                    return JsonConvert.DeserializeObject<UploadResponse>(json)?.Status switch {
                        "unchanged" => UploadStatus.Unchanged,
                        "throttled" => UploadStatus.Throttled,
                        _ => UploadStatus.Stored,
                    };
                }
            }
            catch (JsonException ex) {
                // The upload itself went through, so treat it as stored.
                Logger.Warn("Could not parse the upload response", ex);
                return UploadStatus.Stored;
            }
            catch (WebException ex) {
                var resp = ex.Response != null ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : string.Empty;
                Logger.Warn(ex.Message, ex);
                Logger.Warn(resp);

                return UploadStatus.Failed;
            }
        }

    }
}

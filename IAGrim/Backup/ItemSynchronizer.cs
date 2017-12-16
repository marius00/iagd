using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Models;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IAGrim.Backup {
    public class ItemSynchronizer : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemSynchronizer));
        private BackgroundWorker _bw;
        private readonly IPlayerItemDao _playerItemDao;
        private DateTime _nextUpload;
        private DateTime _nextDownload;
        private DateTime _authenticationDelay;
        private int _authenticationIncrement = 1;
        private readonly string _token;
        private readonly string _host;
        private readonly Action _authenticationIssuesCallback;

        private string UploadUrl => $"{_host}/PlayerItemBackup/Upload";

        private string DownloadUrl => $"{_host}/PlayerItemBackup/Download";

        private string DeleteUrl => $"{_host}/PlayerItemBackup/Delete";

        private string LoginUrl => $"{_host}/PlayerItemBackup/Login";

        private string VerifyCheckUrl => $"{_host}/PlayerItemBackup/IsVerified";

        private string LogoutUrl => $"{_host}/PlayerItemBackup/Logout";

        private string LogoutAllUrl => $"{_host}/PlayerItemBackup/LogoutAll";

        public ItemSynchronizer(IPlayerItemDao playerItemDao, string token, string host, Action authenticationIssuesCallback) {
            this._playerItemDao = playerItemDao;
            this._token = token;
            this._host = host;
            this._nextUpload = DateTime.UtcNow;
            this._nextDownload = DateTime.UtcNow;
            this._authenticationDelay = default(DateTime);
            this._authenticationIssuesCallback = authenticationIssuesCallback;
        }

        public void Start() {
            _bw = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bw.DoWork += new DoWorkEventHandler(Loop);
            _bw.WorkerSupportsCancellation = true;
            _bw.RunWorkerAsync();
        }


        private void HandleAuthenticationIssues(int errorCode) {
            if (errorCode == PlayerItemResponseCodes.ERRORCODE_NOT_AUTHORIZED) {
                _authenticationDelay = DateTime.UtcNow.AddMinutes(_authenticationIncrement);
                _authenticationIncrement = Math.Min(_authenticationIncrement * 2, 45);
                Properties.Settings.Default.OnlineBackupVerified = false;
                Properties.Settings.Default.Save();
                Logger.Warn("Online backup halted, account not yet verified.");
            }
            else if (errorCode == PlayerItemResponseCodes.ERRORCODE_AUTH_FAILURE) {
                Logger.Warn("Authentication failure");
                Properties.Settings.Default.OnlineBackupToken = string.Empty;
                Properties.Settings.Default.Save();
            }
            // TODO CALLBACK

            _authenticationIssuesCallback?.Invoke();
        }

        public string Login(string email, out string error) {
            var param = "email=" + Uri.EscapeDataString(email);
            var result = Post(param, LoginUrl);
            if (result == null) {
                error = "Unknown error";
                return string.Empty;
            }

            PlayerItemLoginResponse obj = JsonConvert.DeserializeObject<PlayerItemLoginResponse>(result);
            if (obj.Success) {
                error = string.Empty;
                Logger.Info("Online backup login successful, waiting for user to confirm email..");
                return obj.Token;                
            } else {
                switch (obj.ErrorCode) {
                    case PlayerItemResponseCodes.ERRORCODE_INVALID_EMAIL:
                        error = "Invalid email address";
                        break;
                    case PlayerItemResponseCodes.ERRORCODE_THROTTLED:
                        error = "Throttled - Please try again later";
                        break;
                    case PlayerItemResponseCodes.ERRORCODE_TRY_AGAIN:
                        error = "An unknown error occurred - Please try again later";
                        break;
                    default:
                        error = "Unhandled error!?";
                        break;
                }

                return string.Empty;
            }
        }



        public bool Verify(string token) {
            var param = "token=" + Uri.EscapeDataString(token);
            var result = Post(param, VerifyCheckUrl);
            return JsonConvert.DeserializeObject<bool>(result);
        }

        public void Logout() {
            var param = "token=" + Uri.EscapeDataString(_token);
            var result = Post(param, LogoutUrl);
        }

        public void LogoutAll() {
            var param = "token=" + Uri.EscapeDataString(_token);
            var result = Post(param, LogoutAllUrl);
        }



        private static PlayerItemBackupUpload Map(PlayerItem item) {
            return new PlayerItemBackupUpload {
                BaseRecord = item.BaseRecord,
                IsHardcore = item.IsHardcore,
                MateriaCombines = (ushort)item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                Mod = item.Mod,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionRecord = item.RelicCompletionBonusRecord,
                RelicSeed = item.RelicSeed,
                Seed = item.Seed,
                StackCount = (UInt16)item.StackCount,
                SuffixRecord = item.SuffixRecord
            };
        }

        private static PlayerItem Map(PlayerItemBackupDownloadItem item) {
            return new Database.PlayerItem {
                BaseRecord = item.BaseRecord,
                Seed = item.Seed,
                RelicSeed = item.RelicSeed,
                PrefixRecord = item.PrefixRecord,
                SuffixRecord = item.SuffixRecord,
                ModifierRecord = item.ModifierRecord,
                MateriaRecord = item.MateriaRecord,
                RelicCompletionBonusRecord = item.RelicCompletionRecord,
                StackCount = item.StackCount,
                MateriaCombines = item.MateriaCombines,
                Mod = item.Mod,
                IsHardcore = item.IsHardcore,
                OnlineId = item.OnlineId
            };
        }

        private void Upload() {
            
            PlayerItem item = null;
            do {
                item = _playerItemDao.GetSingleUnsynchronizedItem();
                if (item != null) {                
                    var simplified = Map(item);
                    var json = JsonConvert.SerializeObject(simplified);
                    var result = Post($"token={_token}&item={json}", UploadUrl);
                    if (string.IsNullOrEmpty(result)) {
                        Logger.Warn("Error uploading items to remote backup server");
                        _nextUpload = DateTime.UtcNow.AddMinutes(30);
                        return;
                    }
                    var obj = JsonConvert.DeserializeObject<PlayerItemBackupUploadResponse>(result);
                    if (!obj.Success) {
                        HandleAuthenticationIssues(obj.ErrorCode);
                        Logger.Warn($"Failed synchronizing item with backup, error code {obj.ErrorCode}");
                        
                    }
                    else {
                        item.OnlineId = obj.OID;
                        _playerItemDao.Update(item);
                        Properties.Settings.Default.LastOnlineBackup = obj.Modified;

                    }
                }
            } while (item != null);
            Properties.Settings.Default.Save();

            _nextUpload = DateTime.UtcNow.AddMinutes(3);
        }



        private void Download() {
            var result = Post($"token={_token}&lastUpdate={Properties.Settings.Default.LastOnlineBackup}", DownloadUrl);
            if (string.IsNullOrEmpty(result)) {
                Logger.Warn("Error downloading items from remote backup server");
                _nextDownload = DateTime.UtcNow.AddMinutes(30);
                return;
            }

            var obj = JsonConvert.DeserializeObject<PlayerItemBackupDownload>(result);
            if (obj.Success) {
                var existingItems = _playerItemDao.ListAll()
                    .Where(m => m.OnlineId.HasValue)
                    .Select(m => m.OnlineId.Value)
                    .ToList();

                var newItems = obj.Items.Where(onlineItem => !existingItems.Contains(onlineItem.OnlineId)).ToList();
                foreach (var simplified in newItems) {
                    var item = Map(simplified);
                    // TODO: Get timestamp from server [on upload]
                    _playerItemDao.Save(item);
                    
                }
                foreach (var deleted in obj.Deleted) {
                    var item = _playerItemDao.GetByOnlineId(deleted);
                    if (item != null)
                        _playerItemDao.Remove(item);
                }

                Logger.Info($"Added {obj.Items.Count} items after online sync");
                Logger.Info($"Removed {obj.Deleted.Count} items after online sync");

                // TODO: This is very low for every-day usage, and very high for frequent pc switch usage
                _nextDownload = DateTime.UtcNow.AddHours(1);

                if (obj.Items.Count > 0) {
                    Properties.Settings.Default.LastOnlineBackup = obj.Items.Max(m => m.ModifiedDate);
                    Properties.Settings.Default.Save();
                }
            }
            else {
                HandleAuthenticationIssues(obj.ErrorCode);
                Logger.Warn($"Could not synchronize items from online backup. Error code {obj.ErrorCode}");
            }
        }


        private void Delete() {

            // TODO: Delete [for deletion, loop while there are items to delete => then sleep)
            var items = _playerItemDao.GetItemsMarkedForOnlineDeletion();

            if (items.Count > 0) {
                var ids = Uri.EscapeDataString(JsonConvert.SerializeObject(items.Select(m => m.OID)));
                var parameters = $"token={_token}&ids={ids}";
                var result = Post(parameters, DeleteUrl);
                if (string.IsNullOrEmpty(result)) {
                    Logger.Warn("Error removing items from remote backup server");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<PlayerItemBackupDeleteResponse>(result);
                if (!obj.Success) {
                    Logger.Warn($"Error {obj.ErrorCode} deleting items from online backup.");
                }
                else {
                    if (_playerItemDao.ClearItemsMarkedForOnlineDeletion() != items.Count) {
                        Logger.Warn($"Not all deletion tags were removed.");
                    }
                    else {
                        Logger.Info("Remove backup server notified of deleted items");
                    }
                }
            }
        }

        private void Loop(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "ItemSynchroniser";

            BackgroundWorker bw = sender as BackgroundWorker;
            try {
                
                while (!bw.CancellationPending) {
                    try {
                        Thread.Sleep(1);
                    }
                    catch (Exception) { }


                    if (_authenticationDelay > DateTime.UtcNow)
                        continue;

                    if (DateTime.UtcNow >= _nextUpload) {
                        Delete();
                        Upload();
                    }

                    if (DateTime.UtcNow >= _nextDownload) {
                        Download();
                    }
                }
            }
            // Hopefully these are just IO exceptions, time will tell.
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }
        }




        private static string Post(string postData, string URL) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            if (request == null) {
                Logger.Warn("Could not create HttpWebRequest");
                return null;
            }
            var encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try {
                using (Stream stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                // threshold
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Info("Failed to upload buddy item data.");
                        return null;
                    }

                    string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    return result;
                }
            }
            catch (WebException ex) {
                if (ex.Status != WebExceptionStatus.NameResolutionFailure && ex.Status != WebExceptionStatus.Timeout) {
                    Logger.Warn(ex.Message);
                }
                else {
                    Logger.Info("Could not resolve DNS for backup server, delaying upload.");
                }
            }
            catch (IOException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

            return null;
        }

        public void Dispose() {
            _bw?.CancelAsync();
        }
    }
}

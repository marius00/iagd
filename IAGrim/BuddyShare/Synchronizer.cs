using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.IO.Compression;
using System.Web;
using EvilsoftCommons;
using log4net;
using IAGrim.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EvilsoftCommons.Exceptions;
using IAGrim.BuddyShare.dto;
using IAGrim.Settings;
using IAGrim.Settings.Dto;


namespace IAGrim.BuddyShare {
    public class Synchronizer {
        static readonly ILog Logger = LogManager.GetLogger(typeof(Synchronizer));
        private const long UploadSynchronizeCooldown = 1000 * 60 * 5;
        private const long DownloadSynchronizeCooldown = 1000 * 60 * 5;

        private long _lastBuddyUpdateLength = 0;
        
        public const string UrlBuddyServer = "http://grimdawn.dreamcrash.org/buddyitems/v2";
        private const string UrlBuddyUpload = UrlBuddyServer + "/update";
        private const string UrlBuddyDownload = UrlBuddyServer + "/getuser/";
        private const string UrlBuddyCreateUser = UrlBuddyServer + "/createuser";

        private readonly ActionCooldown _uploadCooldown = new ActionCooldown(UploadSynchronizeCooldown);
        private readonly ActionCooldown _downloadCooldown = new ActionCooldown(DownloadSynchronizeCooldown);
        private readonly SettingsService _settings;

        public Synchronizer(SettingsService settings) {
            _settings = settings;
        }


        /// <summary>
        /// Upload item data to buddies
        /// Will return false if still on cooldown, or item count is the same as before.
        /// (Will not update if adding one item, and removing another)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UploadBuddyData(SerializedPlayerItems data) {
            if (!string.IsNullOrEmpty(data.Items) && data.UserId > 0) {
                if (data.Items.Length != _lastBuddyUpdateLength) {
                    _lastBuddyUpdateLength = data.Items.Length;

                    string json = JsonConvert.SerializeObject(data);
                    
                    var compressedJson = GzipCompressionHandler.CompressAndConvertToBase64(json);
                    var encodedCompressedJson = HttpUtility.UrlEncode(compressedJson);
                    var result = UploadBuddyData($"json={encodedCompressedJson}", UrlBuddyUpload);

                    // Access denied, we'll need a new user id it seems.
                    // Something must be wrong.
                    if (result != null && result.Status == 401) {
                        _settings.GetPersistent().BuddySyncUserIdV2 = 0;
                        CreateUserId();
                    }

                    return result != null && result.Success;
                }
                else {
                    Logger.Info("Buddy items not updated, no new changes detected.");
                    return true;
                }
            }
            else {
                Logger.Info("Tried to upload buddy data, but either userid or items were null");
            }

            return false;
        }


        /// <summary>
        /// Create / request a user ID
        /// </summary>
        /// <returns></returns>
        public long CreateUserId() {
            
            long existingId = _settings.GetPersistent().BuddySyncUserIdV2 ?? 0L;
            if (existingId != 0)
                return existingId;

            var result = UploadBuddyData($"uuid={RuntimeSettings.Uuid}", UrlBuddyCreateUser, true);
            var json = result.Content;
            var x = JObject.Parse(json);
            if ("ok".Equals(x["status"].ToString())) {
                long id;
                if (long.TryParse(x["uid"].ToString(), out id)) {
                    _settings.GetPersistent().BuddySyncUserIdV2 = id;
                    return id;
                }
            }

            return 0;
        }



        public List<SerializedPlayerItems> DownloadBuddyItems(List<long> buddies) {
            List<SerializedPlayerItems> result = new List<SerializedPlayerItems>();

            if (_downloadCooldown.IsReady) {
                _downloadCooldown.Reset();

                List<string> compressedJsonData = DownloadBuddyData(buddies);

                List<string> jsonData = compressedJsonData
                    .Select(GzipCompressionHandler.DecodeBase64AndDecompress) // For some reason the string result is quoted, strip them.
                    .ToList();

                foreach (string json in jsonData) {
                    if (!string.IsNullOrEmpty(json)) {
                        result.Add(JObject.Parse(json).ToObject<SerializedPlayerItems>());
                    }
                }
                
            }
            return result;
        }

        /// <summary>
        /// Download buddy item data
        /// </summary>
        /// <param name="buddies">List of JSON strings</param>
        /// <returns></returns>
        private List<string> DownloadBuddyData(List<long> buddies) {
            List<string> result = new List<string>();
            
            try {
                using (WebClient client = new WebClient()) {
                    foreach (long id in buddies) {
                        result.Add(client.DownloadString(UrlBuddyDownload + "/" + id));
                    }
                }
            }
            catch (IOException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }
            catch (WebException ex) {
                if (ex.Status != WebExceptionStatus.NameResolutionFailure && ex.Status != WebExceptionStatus.Timeout) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                }
                else {
                    Logger.Info("Could not resolve DNS for buddy server, skipping upload.");
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

            return result;
        }


        private BuddyUploadResponse UploadBuddyData(string postData, string url, bool forced = false) {
            if (_uploadCooldown.IsOnCooldown && !forced) {
                Logger.Info("Buddy data not uploaded, still on cooldown..");
                return null;
            }
            

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null) {
                Logger.Warn("Could not create HttpWebRequest");
                return null;
            }
            var encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            request.Method = "POST";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;

            try {
                using (Stream stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                // threshold
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Info("Failed to upload buddy item data.");
                        return new BuddyUploadResponse {
                            Status = (int)response.StatusCode
                        };
                    }

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    
                    Logger.Debug(responseString);
                    Logger.Info("Uploaded buddy item data.");

                    _uploadCooldown.Reset();
                    return new BuddyUploadResponse {
                        Content = responseString,
                        Status = 200
                    };
                }
            }
            catch (WebException ex) {
                Logger.Info("Failed to upload buddy item data.");

                using (WebResponse response = ex.Response) {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized) {
                        return new BuddyUploadResponse {
                            Status = (int)httpResponse.StatusCode
                        };
                    }
                }

                if (ex.Status != WebExceptionStatus.NameResolutionFailure && ex.Status != WebExceptionStatus.Timeout) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                }
                else {
                    Logger.Info("Could not resolve DNS for buddy server, skipping upload.");
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


    }
}

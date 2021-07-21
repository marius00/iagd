using System;
using System.Collections.Generic;
using System.Net;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Utilities.HelperClasses;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Backup.Cloud.Service {
    /// <summary>
    /// Simple wrapper around RestService to handle requests and exceptions
    /// </summary>
    public class CloudSyncService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CloudSyncService));
        private readonly RestService _restService;

        public CloudSyncService(RestService restService) {
            _restService = restService;
        }

        /// <summary>
        /// Delete an item from the cloud (typically transferred in-game)
        /// </summary>
        /// <param name="items">Items to delete</param>
        /// <returns></returns>
        public bool Delete(List<DeleteItemDto> items) {
            try {
                var json = JsonConvert.SerializeObject(items);
                return _restService.Post(Uris.DeleteItemsUrl, json);
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
        }

        public ItemDownloadDto Get(long lastTimestamp) {
            var url = $"{Uris.DownloadUrl}?ts={lastTimestamp}";
            return _restService.Get<ItemDownloadDto>(url);
        }

        /// <summary>
        /// Permanently delete an online backup user/account
        /// </summary>
        /// <returns></returns>
        public bool DeleteAccount() {
            return _restService.Delete(Uris.DeleteAccountUrl, "{}");
        }

        public bool Save(List<CloudItemDto> items) {
            var json = JsonConvert.SerializeObject(items);
            return _restService.Post(Uris.UploadItemsUrl, json);
        }

        public LimitsDto GetLimitations() {
            var url = Uris.FetchLimitationsUrl;
            return _restService.Get<LimitsDto>(url);
        }
    }
}
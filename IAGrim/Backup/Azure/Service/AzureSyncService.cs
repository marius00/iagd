using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Azure.Constants;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Utilities.HelperClasses;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Backup.Azure.Service {
    class AzureSyncService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AzureSyncService));
        private readonly RestService _restService;

        public AzureSyncService(RestService restService) {
            _restService = restService;
        }

        public List<AzureItemPartitionDto> GetPartitions() {
            return _restService.Get<List<AzureItemPartitionDto>>(AzureUris.FetchPartitionUrl);
        }

        public bool Delete(List<AzureItemDeletionDto> items) {
            try {
                var json = JsonConvert.SerializeObject(items);
                return _restService.Post(AzureUris.DeleteItemsUrl, json);
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
            catch (Exception ex) {
                ExceptionReporter.ReportException(ex, "AzureSyncService");
                return false;
            }
        }

        public ItemDownloadDto GetItems(string partition) {
            var url = $"{AzureUris.FetchItemsInPartitionUrl}?partition={partition}";
            return _restService.Get<ItemDownloadDto>(url);
        }

        public AzureUploadResult Save(List<AzureUploadItem> items) {
            var json = JsonConvert.SerializeObject(items);
            return _restService.Post<AzureUploadResult>(AzureUris.UploadItemsUrl, json);
        }

        public AzureLimitsDto GetLimitations() {
            var url = AzureUris.FetchLimitationsUrl;
            return _restService.Get<AzureLimitsDto>(url);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using IAGrim.Backup.Azure.Constants;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Utilities.HelperClasses;
using Newtonsoft.Json;

namespace IAGrim.Backup.Azure.Service {
    class AzureSyncService {
        private readonly RestService _restService;

        public AzureSyncService(RestService restService) {
            _restService = restService;
        }

        public List<AzureItemPartitionDto> GetPartitions() {
            return _restService.Get<List<AzureItemPartitionDto>>(AzureUris.FetchPartitionUrl);
        }

        public bool Delete(List<AzureItemDeletionDto> items) {
            var json = JsonConvert.SerializeObject(items);
            return _restService.Post(AzureUris.DeleteItemsUrl, json);
        }

        public ItemDownloadDto GetItems(string partition) {
            var url = $"{AzureUris.FetchItemsInPartitionUrl}?partition={partition}";
            return _restService.Get<ItemDownloadDto>(url);
        }

        public List<AzureUploadedItem> Save(List<AzureUploadItem> items) {
            var json = JsonConvert.SerializeObject(items);
            return _restService.Post<List<AzureUploadedItem>>(AzureUris.UploadItemsUrl, json);
        }
    }
}

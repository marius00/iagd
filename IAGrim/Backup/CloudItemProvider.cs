using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Cloud;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Backup.Cloud.Util;
using IAGrim.Database;
using IAGrim.Database.Interfaces;

namespace IAGrim.Backup {
    public class CloudItemProvider : IItemProvider {
        private readonly IPlayerItemDao _playerItemDao;

        public CloudItemProvider(IPlayerItemDao playerItemDao) {
            _playerItemDao = playerItemDao;
        }

        public IList<ItemIdentifierDto> GetItemsMarkedForOnlineDeletion() {
            return _playerItemDao.GetItemsMarkedForOnlineDeletion()
                .Select(item => new ItemIdentifierDto {Id = item.Id})
                .ToList();
        }

        public void ClearItemsMarkedForOnlineDeletion() {
            _playerItemDao.ClearItemsMarkedForOnlineDeletion();
        }

        // Ensures that all items has an identifier for cloud backups
        private void EnsureLegacyCompatible(IList<PlayerItem> items) {
            bool isChanged = false;
            foreach (var item in items.Where(item => string.IsNullOrEmpty(item.CloudId))) {
                var guid = string.IsNullOrEmpty(item.AzureUuid) ? Guid.NewGuid().ToString().Replace("-", "") : item.AzureUuid;
                item.CloudId = guid;
                isChanged = true;
            }

            if (isChanged) {
                _playerItemDao.UpdateCloudId(items);
            }
        }

        public IList<CloudItemDto> GetUnsynchronizedItems() {
            var items = _playerItemDao.GetUnsynchronizedItems();
            
            EnsureLegacyCompatible(items);
            
            return items.Select(ItemConverter.ToUpload).ToList();
        }

        public void MarkAsSynchronized(IList<CloudItemDto> items) {
            _playerItemDao.SetAsSynchronized(items.Select(item => new ItemIdentifierDto{Id = item.Id}).ToList());
        }
    }
}
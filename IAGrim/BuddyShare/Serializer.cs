using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using IAGrim.BuddyShare.dto;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using Newtonsoft.Json;

namespace IAGrim.BuddyShare {
    class Serializer {
        private const string BuddyVerificationId = "FEFE";
        static readonly ILog Logger = LogManager.GetLogger(typeof(Serializer));
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly SettingsService _settings;

        public Serializer(IBuddyItemDao buddyItemDao, IPlayerItemDao playerItemDao, SettingsService settings) {
            this._buddyItemDao = buddyItemDao;
            this._playerItemDao = playerItemDao;
            _settings = settings;
        }

        public SerializedPlayerItems Serialize() {
            
            if (_settings.GetPersistent().BuddySyncUserIdV2 != 0) {
                SerializedPlayerItems s = GetItems();
                s.Description = _settings.GetPersistent().BuddySyncDescription;
                s.UserId = _settings.GetPersistent().BuddySyncUserIdV2 ?? 0L;
                s.UUID = RuntimeSettings.Uuid;
                return s;
            }

            return null;
        }


        /// <summary>
        /// Deserialize and store items retrieved from BuddySync
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeAndSave(SerializedPlayerItems data) {
            if (string.IsNullOrEmpty(data.Items)) {
                Logger.Debug("Either data or items is null, or no items received. Skipping buddy items update");
                return;
            }

            if (data.Verification != BuddyVerificationId) {
                Logger.Warn($"Received verification id \"{data.Verification}\", expected 0x{BuddyVerificationId}. Buddy most likely has an older version.");
                return;
            }

            // Make sure we don't store an empty buddy description, would get interpreted as an owned item :o
            if (string.IsNullOrEmpty(data.Description)) {
                data.Description = $"Buddy {data.UserId}";
            }
            else if (data.Description.Length < 3) {
                data.Description = $"Buddy {data.Description}";
            }

            Logger.Debug($"Deserializing items for {data.UserId}");
            var preFilteredBuddyItems = JsonConvert.DeserializeObject<List<JsonBuddyItem>>(data.Items);
            var buddyItems = preFilteredBuddyItems.Where(m => m.Id > 0).ToList();
            if (buddyItems.Count != preFilteredBuddyItems.Count) {
                Logger.Warn($"{preFilteredBuddyItems.Count - buddyItems.Count} items were filtered out due to a version missmatch");
            }
            
            Logger.Debug($"Storing {buddyItems.Count} items for {data.UserId}");
            _buddyItemDao.SetItems(data.UserId, data.Description, buddyItems);

            Logger.Debug($"Updating item rarity for {data.UserId}");
            var items = _buddyItemDao.ListItemsWithMissingRarity();
            _buddyItemDao.UpdateRarity(items);

            Logger.Debug($"Updating item names for {data.UserId}");
            _buddyItemDao.UpdateNames(_buddyItemDao.ListItemsWithMissingName());

            Logger.Debug($"Updating item level requirements for {data.UserId}");
            _buddyItemDao.UpdateLevelRequirements(_buddyItemDao.ListItemsWithMissingLevelRequirement());

            Logger.Debug($"Buddy sync complete for {data.UserId}");
        }

        private JsonBuddyItem Convert(PlayerItem item) {
            return new JsonBuddyItem {
                Id = item.Id,
                BaseRecord = item.BaseRecord,
                MateriaRecord = item.MateriaRecord,
                Mod = item.Mod,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                StackCount = item.StackCount,
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                IsHardcore = item.IsHardcore,
                CreationDate = item.CreationDate ?? 0L
            };
        }

        /// <summary>
        /// Serializes each player item
        /// </summary>
        /// <returns></returns>
        private SerializedPlayerItems GetItems() {
            var items = _playerItemDao.ListAll().Select(Convert).ToList();
            var serializedItems = JsonConvert.SerializeObject(items);
            return new SerializedPlayerItems {
                Verification = BuddyVerificationId,
                Items = serializedItems
            };
        }
    }
}

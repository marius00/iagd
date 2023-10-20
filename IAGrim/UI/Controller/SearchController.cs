using IAGrim.Database;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Synchronizer;
using IAGrim.Services;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.Settings;

namespace IAGrim.UI.Controller {
    public class SearchController {
        private const int TakeSize = 64;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SearchController));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemStatService _itemStatService;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly ItemPaginationService _itemPaginationService;
        private readonly SettingsService _settings;
        private readonly IItemCollectionDao _itemCollectionRepo;


        public IBrowserCallbacks Browser;
        public readonly JavascriptIntegration JsIntegration = new JavascriptIntegration();
        public event EventHandler OnSearch;

        public SearchController(
            IPlayerItemDao playerItemDao,
            ItemStatService itemStatService,
            IBuddyItemDao buddyItemDao,
            SettingsService settings,
            IItemCollectionDao itemCollectionRepo) {
            _playerItemDao = playerItemDao;
            _itemStatService = itemStatService;
            _itemPaginationService = new ItemPaginationService(TakeSize);
            _buddyItemDao = buddyItemDao;
            _settings = settings;
            _itemCollectionRepo = itemCollectionRepo;

            JsIntegration.OnRequestItems += JsBind_OnRequestItems;
        }

        // TODO: Redo! Infiscroll
        private void JsBind_OnRequestItems(object sender, EventArgs e) {
            ApplyItems(true);
            OnSearch?.Invoke(this, null);
        }

        public string Search(ItemSearchRequest query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel) {
            // Signal that we are loading items
            Browser.ShowLoadingAnimation(true);

            var message = Search_(query, duplicatesOnly, includeBuddyItems, orderByLevel);


            return message;
        }

        private void UpdateCollectionItems(ItemSearchRequest query) {
            Thread thread = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();
                Browser.SetCollectionItems(_itemCollectionRepo.GetItemCollection(query));
            });
            thread.Start();


            Browser.SetCollectionAggregateData(_itemCollectionRepo.GetItemAggregateStats());
        }
        
        private bool ApplyItems(bool append) {
            var items = _itemPaginationService.Fetch();
            if (items.Count == 0) {
                Browser.AddItems(new List<List<JsonItem>>(0));
                return false;
            }
            
            _itemStatService.ApplyStats(items);

            var convertedItems = ItemHtmlWriter.ToJsonSerializable(items);
            if (append) {
                Browser.AddItems(convertedItems);
            }
            else {
                Browser.SetItems(convertedItems, _itemPaginationService.NumItems);
            }

            // UpdateCollectionItems();
            return true;
        }

        private string Search_(ItemSearchRequest query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel) {
            OnSearch?.Invoke(this, null);
            string message;


            Logger.Info("Searching for items..");

            var items = new List<PlayerHeldItem>();

            items.AddRange(_playerItemDao.SearchForItems(query));

            var personalCount = items.Sum(i => (long) i.Count);

            if (includeBuddyItems && !query.SocketedOnly) {
                AddBuddyItems(items, query, out message);
            }
            else {
                message = personalCount == 0
                    ? RuntimeSettings.Language.GetTag("iatag_no_matching_items_found")
                    : string.Empty;
            }

            if (_itemPaginationService.Update(items, orderByLevel)) {
                if (!ApplyItems(false)) {
                    Browser.SetItems(new List<List<JsonItem>>(0), 0);
                }
                UpdateCollectionItems(query);
            }
            else {

                Browser.ShowLoadingAnimation(false);
            }

            return message;
        }

        private void AddBuddyItems(List<PlayerHeldItem> items, ItemSearchRequest query, out string message) {
            var buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            if (buddyItems.Count > 0) {
                items.AddRange(buddyItems);
                message = RuntimeSettings.Language.GetTag("iatag_items_found_self_and_buddy", items.Count - buddyItems.Count, buddyItems.Count);
            } 
            else {
                message = RuntimeSettings.Language.GetTag("iatag_items_found_selfonly", items.Count);
            }
        }

    }
}
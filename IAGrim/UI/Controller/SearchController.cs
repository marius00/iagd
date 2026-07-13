using IAGrim.Database;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using EvilsoftCommons.Exceptions;
using IAGrim.Database.DAO.Util;

namespace IAGrim.UI.Controller {
    public class SearchController {
        private const int TakeSize = 64;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SearchController));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemStatService _itemStatService;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly ItemPaginationService _itemPaginationService;
        private readonly IItemCollectionDao _itemCollectionRepo;


        public IBrowserCallbacks Browser;
        public readonly JavascriptIntegration JsIntegration = new JavascriptIntegration();
        public event EventHandler? OnSearch;

        // The most recent search query, used to (re)build the Collection tab on demand. The Collection
        // view is filtered by the same query as the item search, but is only fetched when that tab is open.
        private ItemSearchRequest? _lastQuery;

        public SearchController(
            IPlayerItemDao playerItemDao,
            ItemStatService itemStatService,
            IBuddyItemDao buddyItemDao,
            IItemCollectionDao itemCollectionRepo) {
            _playerItemDao = playerItemDao;
            _itemStatService = itemStatService;
            _itemPaginationService = new ItemPaginationService(TakeSize);
            _buddyItemDao = buddyItemDao;
            _itemCollectionRepo = itemCollectionRepo;

            JsIntegration.OnRequestItems += JsBind_OnRequestItems;
            JsIntegration.OnRequestCollectionData += JsBind_OnRequestCollectionData;
        }

        private void JsBind_OnRequestCollectionData(object sender, EventArgs e) {
            if (_lastQuery == null) {
                return; // No search has run yet; nothing to build the collection view from.
            }

            UpdateCollectionItems(_lastQuery);
        }

        // TODO: Redo! Infiscroll
        private void JsBind_OnRequestItems(object sender, EventArgs e) {
            ApplyItems(true);
            OnSearch?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateCollectionItems(ItemSearchRequest query) {
            // Both of these feed the side "collection" panel, not the main results grid (already
            // rendered by now). They're heavy full-table aggregates, so keep them entirely off the
            // UI thread — running GetItemAggregateStats on Main was freezing the UI ~2s per search.
            Thread thread = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                var itemCollection = _itemCollectionRepo.GetItemCollection(query);
                Browser.SetCollectionItems(itemCollection, query.IsHardcore);

                var aggregateStats = _itemCollectionRepo.GetItemAggregateStats();
                Browser.SetCollectionAggregateData(aggregateStats);
            });
            thread.Start();
        }

        private bool ApplyItems(bool append) {
            var items = _itemPaginationService.Fetch();

            if (items.Count == 0) {
                Browser.AddItems(new List<List<JsonItem>>(0));
                return false;
            }

            var playerItemsOnPage = items.SelectMany(m => m).OfType<PlayerItem>().ToList();
            _playerItemDao.PopulateReplicaAndPetInfo(playerItemsOnPage);

            _itemStatService.ApplyStats(items.SelectMany(m => m));

            var convertedItems = ItemHtmlWriter.ToJsonSerializable(items);

            if (append) {
                Browser.AddItems(convertedItems);
            }
            else {
                Browser.SetItems(convertedItems, _itemPaginationService.NumTotalItems);
            }

            return true;
        }

        public void Search(ItemSearchRequest query, PlayerItem item) {
            if (!Browser.IsReady()) {
                return;
            }
            Logger.Info("Checking if newly looted item matches filter..");

            var items = _playerItemDao.SearchForItems(query, out _, item);
            _playerItemDao.PopulateReplicaAndPetInfo(items);
            var merged = ItemOperationsUtility.MergeStackSize(items);
            _itemStatService.ApplyStats(merged.SelectMany(m => m));
            var convertedItems = ItemHtmlWriter.ToJsonSerializable(merged);
            Browser.AddItems(convertedItems);
        }

        public string Search(ItemSearchRequest query, bool includeBuddyItems, bool orderByLevel) {
            if (!Browser.IsReady()) {
                return string.Empty;
            }
            OnSearch?.Invoke(this, EventArgs.Empty);
            string message;

            // Signal that we are loading items
            Browser.ShowLoadingAnimation(true);
            try {

                Logger.Info("Searching for items..");

                // Remember the query so the Collection tab can be (re)built on demand when the user opens it.
                _lastQuery = query;

                var swTotal = System.Diagnostics.Stopwatch.StartNew();

                var items = new List<PlayerHeldItem>();
                items.AddRange(_playerItemDao.SearchForItems(query, out bool wasTruncated));

                var personalCount = items.Sum(i => (long)i.Count);

                if (includeBuddyItems && !query.SocketedOnly) {
                    AddBuddyItems(items, query, wasTruncated, out message);
                }
                else {
                    message = personalCount == 0
                        ? RuntimeSettings.Language.GetTag("iatag_no_matching_items_found")
                        : string.Empty;
                }

                var merged = ItemOperationsUtility.MergeStackSize(items);

                if (_itemPaginationService.Update(merged, orderByLevel, items.Count)) {
                    if (!ApplyItems(false)) {
                        Browser.SetItems(new List<List<JsonItem>>(0), 0);
                    }

                    // Collection data is no longer fetched here on every search. The frontend requests it
                    // (via RequestCollectionData) only when the Collection tab is actually open.
                }
                else {
                    Browser.ShowLoadingAnimation(false);
                }

                // We have no search filters, yet can barely find any items. Despite there being more than twice as many items as we found.
                // This might indicate the mod filter dropdown has the wrong setting.
                var numOtherItems = _playerItemDao.GetNumItems() - personalCount;
                if (query.IsEmpty && personalCount < 300 && numOtherItems > personalCount) {
                    Browser.ShowModFilterWarning((int)numOtherItems);
                }

                swTotal.Stop();
                Logger.Info($"Search completed in {swTotal.ElapsedMilliseconds}ms, {items.Count} results{(wasTruncated ? $" (capped at {PlayerItemDaoImpl.MaxSearchResults})" : "")}");

                return message;
            }
            finally {
                Browser.ShowLoadingAnimation(false);
            }
        }

        private static object FormatCount(int count, bool wasTruncated) => wasTruncated ? $"{count}+" : count;

        private void AddBuddyItems(List<PlayerHeldItem> items, ItemSearchRequest query, bool wasTruncated, out string message) {
            var buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            if (buddyItems.Count > 0) {
                items.AddRange(buddyItems);
                message = RuntimeSettings.Language.GetTag("iatag_items_found_self_and_buddy", FormatCount(items.Count - buddyItems.Count, wasTruncated), buddyItems.Count);
            }
            else {
                message = RuntimeSettings.Language.GetTag("iatag_items_found_selfonly", FormatCount(items.Count, wasTruncated));
            }
        }

    }
}
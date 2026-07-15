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


        public IBrowserCallbacks? Browser;
        public readonly JavascriptIntegration JsIntegration = new JavascriptIntegration();
        public event EventHandler? OnSearch;

        // The most recent search query, used to (re)build the Collection tab on demand. The Collection
        // view is filtered by the same query as the item search, but is only fetched when that tab is open.
        private ItemSearchRequest? _lastQuery;

        // Cross-batch pagination state for the current query. The DB search is capped at MaxSearchResults
        // rows per call; once the user scrolls past the buffered page we fetch the next DB page at _dbSkip.
        private bool _lastOrderByLevel;
        private int _dbSkip;                 // Offset of the next (not-yet-fetched) DB page of player items.
        private int _playerTotalCount;       // Exact total matching player items once known; while deferred, the floor (MaxSearchResults).
        private bool _playerTotalKnown;      // False while the exact total is deferred (result was capped and COUNT skipped).
        private bool _lastDbPageFull;        // The most recent DB page came back full, so more player items may remain.
        private int _buddyCount;             // Buddy items are fetched once up front and never paginated.

        // Are there more player items to fetch from the DB? If we know the exact total, compare against it;
        // otherwise fall back to "the last DB page came back full" (short-page detection), which lets
        // pagination work without paying for an up-front COUNT on the first page.
        private bool MorePlayerPages => _playerTotalKnown ? _dbSkip < _playerTotalCount : _lastDbPageFull;

        // Whether the frontend should keep requesting more items: either the buffer still holds unserved
        // rows, or there are further DB pages to fetch.
        private bool HasMore => !_itemPaginationService.BufferExhausted || MorePlayerPages;

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

        private void JsBind_OnRequestCollectionData(object? sender, EventArgs e) {
            if (_lastQuery == null) {
                return; // No search has run yet; nothing to build the collection view from.
            }

            UpdateCollectionItems(_lastQuery);
        }

        // TODO: Redo! Infiscroll
        private void JsBind_OnRequestItems(object? sender, EventArgs e) {
            ApplyItems(true);
            OnSearch?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateCollectionItems(ItemSearchRequest query) {
            var browser = Browser;
            if (browser == null) {
                return;
            }

            // Both of these feed the side "collection" panel, not the main results grid (already
            // rendered by now). They're heavy full-table aggregates, so keep them entirely off the
            // UI thread — running GetItemAggregateStats on Main was freezing the UI ~2s per search.
            Thread thread = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                var itemCollection = _itemCollectionRepo.GetItemCollection(query);
                browser.SetCollectionItems(itemCollection, query.IsHardcore);

                var aggregateStats = _itemCollectionRepo.GetItemAggregateStats();
                browser.SetCollectionAggregateData(aggregateStats);
            });
            thread.Start();
        }

        private bool ApplyItems(bool append) {
            var browser = Browser;
            if (browser == null) {
                return false;
            }

            // When appending (infinite scroll) and the in-memory buffer is drained, pull the next DB page
            // (items 1001-2000, 2001-3000, ...) before serving the next UI batch. Buddy items are not
            // paginated (fetched once up front), so only player items are fetched here.
            int updatedNumItemsFound = -1;
            if (append && _lastQuery != null && _itemPaginationService.BufferExhausted && MorePlayerPages) {
                // The user has scrolled past the first page, so the exact total is now worth computing (if we
                // deferred it). We fold that COUNT into this next-page fetch: it runs at most once per search,
                // and only for the minority of searches the user actually paginates through.
                bool computeCount = !_playerTotalKnown;

                var more = _playerItemDao.SearchForItems(_lastQuery, _dbSkip, _lastOrderByLevel, computeCount, out int total, out bool moreTruncated);
                _dbSkip += PlayerItemDaoImpl.MaxSearchResults;
                _lastDbPageFull = moreTruncated;

                if (computeCount && total != PlayerItemDaoImpl.UnknownTotalCount) {
                    _playerTotalCount = total;
                    _playerTotalKnown = true;
                    updatedNumItemsFound = _playerTotalCount + _buddyCount;
                }

                _itemPaginationService.Append(ItemOperationsUtility.MergeStackSize(more));
            }

            var items = _itemPaginationService.Fetch();

            if (items.Count == 0) {
                browser.AddItems(new List<List<JsonItem>>(0), HasMore);
                return false;
            }

            var playerItemsOnPage = items.SelectMany(m => m).OfType<PlayerItem>().ToList();
            _playerItemDao.PopulateReplicaAndPetInfo(playerItemsOnPage);

            _itemStatService.ApplyStats(items.SelectMany(m => m));

            var convertedItems = ItemHtmlWriter.ToJsonSerializable(items);

            if (append) {
                browser.AddItems(convertedItems, HasMore, updatedNumItemsFound);
            }
            else {
                browser.SetItems(convertedItems, _itemPaginationService.NumTotalItems, HasMore, !_playerTotalKnown);
            }

            return true;
        }

        public void Search(ItemSearchRequest query, PlayerItem item) {
            var browser = Browser;
            if (browser == null || !browser.IsReady()) {
                return;
            }
            Logger.Info("Checking if newly looted item matches filter..");

            var items = _playerItemDao.SearchForItems(query, 0, false, false, out _, out _, item);
            _playerItemDao.PopulateReplicaAndPetInfo(items);
            var merged = ItemOperationsUtility.MergeStackSize(items);
            _itemStatService.ApplyStats(merged.SelectMany(m => m));
            var convertedItems = ItemHtmlWriter.ToJsonSerializable(merged);
            browser.AddItems(convertedItems, HasMore);
        }

        public string Search(ItemSearchRequest query, bool includeBuddyItems, bool orderByLevel) {
            var browser = Browser;
            if (browser == null || !browser.IsReady()) {
                return string.Empty;
            }
            OnSearch?.Invoke(this, EventArgs.Empty);
            string message;

            // Signal that we are loading items
            browser.ShowLoadingAnimation(true);
            try {

                Logger.Info("Searching for items..");

                // Remember the query so the Collection tab can be (re)built on demand when the user opens it.
                _lastQuery = query;

                var swTotal = System.Diagnostics.Stopwatch.StartNew();

                // Reset cross-batch pagination state for this new query and fetch the first DB page.
                _lastOrderByLevel = orderByLevel;
                _dbSkip = PlayerItemDaoImpl.MaxSearchResults; // first page is offset 0; next page starts here
                _buddyCount = 0;

                // Defer the exact total on the first page (computeCount: false). If the result is capped we show
                // "1000+" and only pay for the (full-scan) COUNT if/when the user actually paginates past it.
                var items = new List<PlayerHeldItem>();
                items.AddRange(_playerItemDao.SearchForItems(query, 0, orderByLevel, false, out int playerTotal, out bool wasTruncated));

                _playerTotalKnown = !wasTruncated;
                _lastDbPageFull = wasTruncated;
                // When the exact total is known, use it; otherwise hold the floor (MaxSearchResults) for display.
                _playerTotalCount = _playerTotalKnown ? playerTotal : PlayerItemDaoImpl.MaxSearchResults;

                var personalCount = items.Sum(i => (long)i.Count);

                if (includeBuddyItems && !query.SocketedOnly) {
                    AddBuddyItems(items, query, out message);
                }
                else {
                    message = _playerTotalKnown && playerTotal == 0
                        ? RuntimeSettings.Language!.GetTag("iatag_no_matching_items_found")
                        : string.Empty;
                }

                var merged = ItemOperationsUtility.MergeStackSize(items);

                if (_itemPaginationService.Update(merged, orderByLevel, _playerTotalCount + _buddyCount)) {
                    if (!ApplyItems(false)) {
                        browser.SetItems(new List<List<JsonItem>>(0), 0, false);
                    }

                    // Collection data is no longer fetched here on every search. The frontend requests it
                    // (via RequestCollectionData) only when the Collection tab is actually open.
                }
                else {
                    browser.ShowLoadingAnimation(false);
                }

                // We have no search filters, yet can barely find any items. Despite there being more than twice as many items as we found.
                // This might indicate the mod filter dropdown has the wrong setting.
                var numOtherItems = _playerItemDao.GetNumItems() - personalCount;
                if (query.IsEmpty && personalCount < 300 && numOtherItems > personalCount) {
                    browser.ShowModFilterWarning((int)numOtherItems);
                }

                swTotal.Stop();
                Logger.Info($"Search completed in {swTotal.ElapsedMilliseconds}ms, {PlayerTotalDisplay} total results (first page of up to {PlayerItemDaoImpl.MaxSearchResults})");

                return message;
            }
            finally {
                browser.ShowLoadingAnimation(false);
            }
        }

        // The player total for display: the exact number once known, otherwise "1000+" while it's still
        // deferred (the result was capped and we haven't computed the real COUNT yet).
        private object PlayerTotalDisplay => _playerTotalKnown ? _playerTotalCount : $"{PlayerItemDaoImpl.MaxSearchResults}+";

        private void AddBuddyItems(List<PlayerHeldItem> items, ItemSearchRequest query, out string message) {
            var buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            _buddyCount = buddyItems.Count;
            if (buddyItems.Count > 0) {
                items.AddRange(buddyItems);
                // PlayerTotalDisplay reflects the total across all DB pages (or "1000+" while deferred), not
                // just the first page currently in memory, so the "found" message shows the real count.
                message = RuntimeSettings.Language!.GetTag("iatag_items_found_self_and_buddy", PlayerTotalDisplay, buddyItems.Count);
            }
            else {
                message = RuntimeSettings.Language!.GetTag("iatag_items_found_selfonly", PlayerTotalDisplay);
            }
        }

    }
}
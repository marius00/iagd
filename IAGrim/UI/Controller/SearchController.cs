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
        private readonly bool useCache = false; // Temporarily disabled
        private readonly IDatabaseItemDao _dbItemDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemStatService _itemStatService;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly ItemPaginationService _itemPaginationService;
        private readonly IAugmentationItemDao _augmentationItemRepo;
        private readonly SettingsService _settings;
        private readonly IItemCollectionDao _itemCollectionRepo;


        public IBrowserCallbacks Browser;
        public readonly JavascriptIntegration JsIntegration = new JavascriptIntegration();
        public event EventHandler OnSearch;

        public SearchController(
            IDatabaseItemDao databaseItemDao,
            IPlayerItemDao playerItemDao,
            ItemStatService itemStatService,
            IBuddyItemDao buddyItemDao,
            IAugmentationItemDao augmentationItemRepo,
            SettingsService settings,
            IItemCollectionDao itemCollectionRepo) {
            _dbItemDao = databaseItemDao;
            _playerItemDao = playerItemDao;
            _itemStatService = itemStatService;
            _itemPaginationService = new ItemPaginationService(TakeSize);
            _buddyItemDao = buddyItemDao;
            _augmentationItemRepo = augmentationItemRepo;
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
            Browser.ShowLoadingAnimation();

            var message = Search_(query, duplicatesOnly, includeBuddyItems, orderByLevel);

            if (!ApplyItems(false)) {
                Browser.SetItems(new List<JsonItem>(), 0);
                UpdateCollectionItems();
            }

            return message;
        }

        private void UpdateCollectionItems() {
            Thread thread = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();
                Browser.SetCollectionItems(_itemCollectionRepo.GetItemCollection());
            });
            thread.Start();
        }

        private bool ApplyItems(bool append) {
            var items = _itemPaginationService.Fetch();
            if (items.Count == 0) {
                Browser.AddItems(new List<JsonItem>());
                return false;
            }

            // TODO: For player items, use cached stats if present

            _itemStatService.ApplyStats(items, useCache);

            var convertedItems = ItemHtmlWriter.ToJsonSerializable(items, useCache);
            if (append) {
                Browser.AddItems(convertedItems);
            }
            else {
                Browser.SetItems(convertedItems, _itemPaginationService.NumItems);
            }

            UpdateCollectionItems();
            return true;
        }

        private string Search_(ItemSearchRequest query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel) {
            OnSearch?.Invoke(this, null);
            string message;


            Logger.Info("Searching for items..");

            var items = new List<PlayerHeldItem>();

            items.AddRange(_playerItemDao.SearchForItems(query));

            // This specific filter was easiest to add after the actual search
            // Obs: the "duplicates only" search only works when merging duplicates
            if (duplicatesOnly) {
                items = items.Where(m => m.Count > 1).ToList();
            }

            var personalCount = items.Sum(i => (long) i.Count);

            if (includeBuddyItems && !query.SocketedOnly) {
                AddBuddyItems(items, query, out message);
            }
            else {
                message = personalCount == 0
                    ? RuntimeSettings.Language.GetTag("iatag_no_matching_items_found")
                    : string.Empty;
            }

            if (_settings.GetPersistent().ShowRecipesAsItems && !query.SocketedOnly) {
                AddRecipeItems(items, query);
            }

            if (_settings.GetPersistent().ShowAugmentsAsItems && !query.SocketedOnly) {
                AddAugmentItems(items, query);
            }

            _itemPaginationService.Update(items, orderByLevel);

            return message;
        }

        private void AddBuddyItems(List<PlayerHeldItem> items, ItemSearchRequest query, out string message) {
            var buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            var itemsWithBuddy = items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord));

            foreach (var playerItem in items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord))) {
                foreach (PlayerHeldItem buddyItem in buddyItems.FindAll(buddyItem => buddyItem.Equals(playerItem))) {
                    var buddyName = System.Web.HttpUtility.HtmlEncode(buddyItem.Stash);
                    if (!playerItem.Buddies.Exists(name => name == buddyName)) {
                        playerItem.Buddies.Add(buddyName);
                    }
                }
            }

            Logger.Debug($"Merged {itemsWithBuddy.Count} buddy items into player items");

            // TODO: This should use .Except(), find out why its not working with .Except()
            var remainingBuddyItems = buddyItems.FindAll(buddy => itemsWithBuddy.All(item => !buddy.Equals(item)));

            //We add the Owner name from pure BuddyItems from Stash to BuddyNames
            foreach (PlayerHeldItem remainingBuddyItem in remainingBuddyItems) {
                var buddyName = System.Web.HttpUtility.HtmlEncode(remainingBuddyItem.Stash);
                remainingBuddyItem.Buddies.Add(buddyName);
            }

            //We see if of the remaining Items there are any items with more than one Buddy and merge them
            var multiBuddyItems = remainingBuddyItems
                .FindAll(item => remainingBuddyItems.FindAll(buddy => buddy.BaseRecord == item.BaseRecord).Count() > 1);
            foreach (PlayerHeldItem multiBuddyItem in multiBuddyItems) {
                foreach (PlayerHeldItem item in remainingBuddyItems.FindAll(item => multiBuddyItem.BaseRecord == item.BaseRecord)) {
                    var buddyName = System.Web.HttpUtility.HtmlEncode(item.Stash);
                    if (!multiBuddyItem.Buddies.Exists(name => name == buddyName)) {
                        multiBuddyItem.Buddies.Add(buddyName);
                    }
                }
            }

            var buddyPlayerHeldItems = new List<PlayerHeldItem>(remainingBuddyItems);
            if (buddyPlayerHeldItems.Count > 0) {
                MergeDuplicates(buddyPlayerHeldItems);
                items.AddRange(buddyPlayerHeldItems);
                message = RuntimeSettings.Language.GetTag("iatag_items_found_self_and_buddy", items.Count, buddyPlayerHeldItems.Count);
            } 
            else {
                message = RuntimeSettings.Language.GetTag("iatag_items_found_selfonly", items.Count);
            }
        }

        private void AddAugmentItems(List<PlayerHeldItem> items, ItemSearchRequest query) {
            var augments = _augmentationItemRepo.Search(query);
            var remainingRecipes = augments.Where(recipe => items.All(item => item.BaseRecord != recipe.BaseRecord));
            items.AddRange(remainingRecipes);
        }

        private void AddRecipeItems(List<PlayerHeldItem> items, ItemSearchRequest query) {
            var recipes = _dbItemDao.SearchForRecipeItems(query);

            var itemsWithRecipe = items.FindAll(item => recipes.Any(recipe => recipe.BaseRecord == item.BaseRecord));
            foreach (var item in items) {
                if (itemsWithRecipe.Any(recipe => recipe.BaseRecord == item.BaseRecord)) {
                    item.HasRecipe = true;
                }
            }

            // TODO: This should use .Except(), find out why its not working with .Except()
            var remainingRecipes = recipes.Where(recipe => itemsWithRecipe.All(item => item.BaseRecord != recipe.BaseRecord));
            items.AddRange(remainingRecipes);
        }

        /// <summary>
        /// Merge player held items
        /// IFF MergeDuplicates is enabled in the settings
        /// </summary>
        /// <param name="items"></param>
        private void MergeDuplicates(List<PlayerHeldItem> items) {
            if (!_settings.GetPersistent().MergeDuplicates) {
                return;
            }

            var itemMap = new Dictionary<string, PlayerHeldItem>();

            foreach (var item in items) {
                if (!item.IsKnown) {
                    continue;
                }

                var key = item.Name + item.Stash;

                if (itemMap.ContainsKey(key)) {
                    itemMap[key].Count += item.Count;
                }
                else {
                    itemMap[key] = item;
                }
            }

            items.RemoveAll(m => m.IsKnown);
            items.AddRange(itemMap.Values);
        }
    }
}
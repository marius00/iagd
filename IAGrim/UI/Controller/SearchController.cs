using IAGrim.Database;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Synchronizer;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAGrim.UI.Controller
{

    internal class SearchController
    {
        private const int TakeSize = 64;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SearchController));

        private readonly IDatabaseItemDao _dbItemDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemStatService _itemStatService;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly ItemPaginatorService _itemPaginatorService;
        private readonly TransferStashService _transferStashService;
        private readonly AugmentationItemRepo _augmentationItemRepo;

        private string _previousMod = string.Empty;

        public CefBrowserHandler Browser;
        public readonly JSWrapper JsBind = new JSWrapper { IsTimeToShowNag = -1 };
        public event EventHandler OnSearch;

        public SearchController(
            IDatabaseItemDao databaseItemDao,
            IPlayerItemDao playerItemDao,
            IDatabaseItemStatDao databaseItemStatDao,
            IItemSkillDao itemSkillDao,
            IBuddyItemDao buddyItemDao,
            TransferStashService transferStashService,
            AugmentationItemRepo augmentationItemRepo
        )
        {
            _dbItemDao = databaseItemDao;
            _playerItemDao = playerItemDao;
            _itemStatService = new ItemStatService(databaseItemStatDao, itemSkillDao);
            _itemPaginatorService = new ItemPaginatorService(TakeSize);
            _buddyItemDao = buddyItemDao;
            _transferStashService = transferStashService;
            _augmentationItemRepo = augmentationItemRepo;

            // Just make sure it writes .css/.html files before displaying anything to the browser
            // 
            ItemHtmlWriter.ToJsonSerializeable(new List<PlayerHeldItem>()); // TODO: is this not a NOOP?
            JsBind.OnRequestItems += JsBind_OnRequestItems;
        }

        private void JsBind_OnRequestItems(object sender, EventArgs e)
        {
            if (!JsBind.ItemSourceExhausted)
            {
                if (ApplyItems())
                {
                    Browser.AddItems();
                }
            }

            OnSearch?.Invoke(this, null);
        }

        public string Search(ItemSearchRequest query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel)
        {
            JsBind.ItemSourceExhausted = false;

            // Signal that we are loading items
            Browser.ShowLoadingAnimation();

            var message = Search_(query, duplicatesOnly, includeBuddyItems, orderByLevel);

            if (ApplyItems())
            {
                Browser.RefreshItems();
            }
            else
            {
                JsBind.UpdateItems(new List<JsonItem>());
                Browser.RefreshItems();
            }

            return message;
        }

        private bool ApplyItems()
        {
            var items = _itemPaginatorService.Fetch();
            if (items.Count == 0)
            {
                JsBind.ItemSourceExhausted = true;
                return false;
            }

            _itemStatService.ApplyStats(items);

            var convertedItems = ItemHtmlWriter.ToJsonSerializeable(items);
            JsBind.UpdateItems(convertedItems);
            return true;
        }

        private string Search_(ItemSearchRequest query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel)
        {
            OnSearch?.Invoke(this, null);
            string message;
            long personalCount = 0;


            Logger.Info("Searching for items..");

            var items = new List<PlayerHeldItem>();

            items.AddRange(_playerItemDao.SearchForItems(query));

            // This specific filter was easiest to add after the actual search
            // Obs: the "duplicates only" search only works when merging duplicates
            if (duplicatesOnly)
            {
                items = items.Where(m => m.Count > 1).ToList();
            }

            personalCount = items.Sum(i => i.Count);

            if (includeBuddyItems && !query.SocketedOnly)
            {
                AddBuddyItems(items, query, out message);
            }
            else
            {
                message = personalCount == 0
                    ? GlobalSettings.Language.GetTag("iatag_no_matching_items_found")
                    : string.Empty;
            }

            if (Properties.Settings.Default.ShowRecipesAsItems && !query.SocketedOnly)
            {
                AddRecipeItems(items, query);
            }

            if (Properties.Settings.Default.ShowAugmentsAsItems && !query.SocketedOnly)
            {
                AddAugmentItems(items, query);
            }

            _itemPaginatorService.Update(items, orderByLevel);
            JsBind.ItemSourceExhausted = items.Count == 0;

            return message;
        }

        private void AddBuddyItems(List<PlayerHeldItem> items, ItemSearchRequest query, out string message)
        {
            var buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            var itemsWithBuddy = items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord));

            foreach (var item in items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord)))
            {
                foreach (PlayerHeldItem buddyItem in buddyItems.FindAll(buddy => buddy.BaseRecord == item.BaseRecord))
                {
                    var buddyName = System.Web.HttpUtility.HtmlEncode(buddyItem.Stash);
                    if (!item.Buddies.Exists(name => name == buddyName))
                    {
                        item.Buddies.Add(buddyName);
                    }
                }
            }
            Logger.Debug($"Merged {itemsWithBuddy.Count} buddy items into player items");

            // TODO: This should use .Except(), find out why its not working with .Except()
            var remainingBuddyItems = buddyItems
                .FindAll(buddy => itemsWithBuddy.All(item => item.BaseRecord != buddy.BaseRecord));

            //We add the Owner name from pure BuddyItems from Stash to BuddyNames
            foreach (PlayerHeldItem remainingBuddyItem in remainingBuddyItems)
            {
                var buddyName = System.Web.HttpUtility.HtmlEncode(remainingBuddyItem.Stash);
                remainingBuddyItem.Buddies.Add(buddyName);
            }

            //We see if of the remaining Items there are any items with more than one Buddy and merge them
            var multiBuddyItems = remainingBuddyItems
                .FindAll(item => remainingBuddyItems.FindAll(buddy => buddy.BaseRecord == item.BaseRecord).Count() > 1);
            foreach (PlayerHeldItem multiBuddyItem in multiBuddyItems)
            {
                foreach (PlayerHeldItem item in remainingBuddyItems.FindAll(item => multiBuddyItem.BaseRecord == item.BaseRecord))
                {
                    var buddyName = System.Web.HttpUtility.HtmlEncode(item.Stash);
                    if (!multiBuddyItem.Buddies.Exists(name => name == buddyName))
                    {
                        multiBuddyItem.Buddies.Add(buddyName);
                    }
                }
            }

            var buddyPlayerHeldItems = new List<PlayerHeldItem>(remainingBuddyItems);
            if (buddyPlayerHeldItems.Count > 0)
            {
                MergeDuplicates(buddyPlayerHeldItems);
                items.AddRange(buddyPlayerHeldItems);
                message = GlobalSettings.Language.GetTag("iatag_additional_items_found", buddyPlayerHeldItems.Count);
            }
            else
            {
                message = string.Empty;
            }
        }

        private void AddAugmentItems(List<PlayerHeldItem> items, ItemSearchRequest query)
        {
            var augments = _augmentationItemRepo.Search(query);
            var remainingRecipes = augments.Where(recipe => items.All(item => item.BaseRecord != recipe.BaseRecord));
            items.AddRange(remainingRecipes);
        }

        private void AddRecipeItems(List<PlayerHeldItem> items, ItemSearchRequest query)
        {
            var recipes = _dbItemDao.SearchForRecipeItems(query);

            var itemsWithRecipe = items.FindAll(item => recipes.Any(recipe => recipe.BaseRecord == item.BaseRecord));
            foreach (var item in items)
            {
                if (itemsWithRecipe.Any(recipe => recipe.BaseRecord == item.BaseRecord))
                {
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
        private void MergeDuplicates(List<PlayerHeldItem> items)
        {
            if (!Properties.Settings.Default.MergeDuplicates)
            {
                return;
            }

            var itemMap = new Dictionary<string, PlayerHeldItem>();

            foreach (var item in items)
            {
                if (!item.IsKnown)
                {
                    continue;
                }

                var key = item.Name + item.Stash;

                if (itemMap.ContainsKey(key))
                {
                    itemMap[key].Count += item.Count;
                }
                else
                {
                    itemMap[key] = item;
                }
            }

            items.RemoveAll(m => m.IsKnown);
            items.AddRange(itemMap.Values);
        }

    }
}
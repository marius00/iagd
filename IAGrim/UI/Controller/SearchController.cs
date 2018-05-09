using System;
using CefSharp;
using CefSharp.WinForms;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.UI.Misc;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using IAGrim.Database;
using IAGrim.Database.Synchronizer;
using IAGrim.Parsers.Arz;
using IAGrim.Services.Crafting;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc.CEF;

namespace IAGrim.UI.Controller {

    internal class SearchController {
        private static ILog _logger = LogManager.GetLogger(typeof(SearchController));
        private readonly IDatabaseItemDao _dbItemDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemStatService _itemStatService;
        private readonly IBuddyItemDao _buddyItemDao;
        private const int TakeSize = 100;
        private readonly ItemPaginatorService _itemPaginatorService;
        private readonly RecipeService _recipeService;
        private readonly CostCalculationService _costCalculationService;
        private string _previousMod = string.Empty;
        private readonly StashManager _stashManager;
        private readonly AugmentationItemRepo _augmentationItemRepo;

        public CefBrowserHandler Browser;

        public readonly JSWrapper JsBind = new JSWrapper { IsTimeToShowNag = -1 };
        public event EventHandler OnSearch;

        private string _previousRecipe;
        private string _previousCallback;

        public SearchController(
            IDatabaseItemDao databaseItemDao, 
            IPlayerItemDao playerItemDao, 
            IDatabaseItemStatDao databaseItemStatDao,
            IItemSkillDao itemSkillDao,
            IBuddyItemDao buddyItemDao,
            StashManager stashManager, 
            AugmentationItemRepo augmentationItemRepo
        ) {
            this._dbItemDao = databaseItemDao;
            this._playerItemDao = playerItemDao;
            this._itemStatService = new ItemStatService(databaseItemStatDao, itemSkillDao);
            this._itemPaginatorService = new ItemPaginatorService(TakeSize);
            this._recipeService = new RecipeService(databaseItemDao);
            this._costCalculationService = new CostCalculationService(playerItemDao, stashManager);
            this._buddyItemDao = buddyItemDao;
            this._stashManager = stashManager;
            this._augmentationItemRepo = augmentationItemRepo;


            // Just make sure it writes .css/.html files before displaying anything to the browser
            // 
            ItemHtmlWriter.ToJsonSerializeable(new List<PlayerHeldItem>()); // TODO: is this not a NOOP?
            JsBind.OnRequestItems += JsBind_OnRequestItems;

            // Return the ingredients for a given recipe
            JsBind.OnRequestRecipeIngredients += (sender, args) => {
                var recipeArgument = args as RequestRecipeArgument;
                var ingredients = _recipeService.GetRecipeIngredients(recipeArgument?.RecipeRecord);
                _costCalculationService.Populate(ingredients);
                _costCalculationService.SetMod(_previousMod);

                _previousCallback = recipeArgument?.Callback;
                _previousRecipe = recipeArgument?.RecipeRecord;
                Browser.SetRecipeIngredients(JsBind.Serialize(ingredients));
            };


            // Update the recipe when the stash has changed
            stashManager.StashUpdated += StashManagerOnStashUpdated;


            // Return the list of recipes
            JsBind.OnRequestRecipeList += (sender, args) => {
                var recipes = _recipeService.GetRecipeList();
                Browser.SetRecipes(JsBind.Serialize(recipes));
            };
        }

        private void StashManagerOnStashUpdated(object o, EventArgs eventArgs) {
            if (!string.IsNullOrEmpty(_previousRecipe)) {
                var ingredients = _recipeService.GetRecipeIngredients(_previousRecipe);
                _costCalculationService.Populate(ingredients);
                _costCalculationService.SetMod(_previousMod);
                Browser.SetRecipeIngredients(JsBind.Serialize(ingredients));
            }
        }

        ~SearchController() {
            _stashManager.StashUpdated -= StashManagerOnStashUpdated;
        }


        private void JsBind_OnRequestItems(object sender, System.EventArgs e) {

            if (!JsBind.ItemSourceExhausted) {
                if (ApplyItems()) {
                    Browser.AddItems();
                }
            }

            OnSearch?.Invoke(this, null);
        }

        public string Search(Search query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel) {
            JsBind.ItemSourceExhausted = false;

            // Signal that we are loading items
            Browser.ShowLoadingAnimation();

            var message = Search_(query, duplicatesOnly, includeBuddyItems, orderByLevel);

            if (ApplyItems()) {
                Browser.RefreshItems();
            }
            else {
                JsBind.UpdateItems(new List<JsonItem>());
                Browser.RefreshItems();
            }

            return message;
        }

        private bool ApplyItems() {
            var items = _itemPaginatorService.Fetch();
            if (items.Count == 0) {
                JsBind.ItemSourceExhausted = true;
                return false;
            }
            else {
                _itemStatService.ApplyStats(items);

                List<JsonItem> convertedItems = ItemHtmlWriter.ToJsonSerializeable(items);
                JsBind.UpdateItems(convertedItems);
                return true;
            }
        }

        private string Search_(Search query, bool duplicatesOnly, bool includeBuddyItems, bool orderByLevel) {
            OnSearch?.Invoke(this, null);
            string message;
            long personalCount = 0;

            // If we've changed the mod, we'll need to update any recipes. (data source changes)
            if (_previousCallback != query?.Mod) {
                _previousMod = query?.Mod;
                StashManagerOnStashUpdated(null, null);
            }

            _logger.Info("Searching for items..");

            List<PlayerHeldItem> items = new List<PlayerHeldItem>();
            items.AddRange(_playerItemDao.SearchForItems(query));
           

            // This specific filter was easiest to add after the actual search
            // Obs: the "duplicates only" search only works when merging duplicates
            if (duplicatesOnly) {
                items = items.Where(m => m.Count > 1).ToList();
            }

            
            personalCount = items.Sum(i => i.Count);

            if (includeBuddyItems && !query.SocketedOnly) {
                AddBuddyItems(items, query, out message);
            }
            else {
                if (personalCount == 0)
                    message = "No matching items found";
                else {
                    message = string.Empty;
                }
            }

            if ((bool)Properties.Settings.Default.ShowRecipesAsItems && !query.SocketedOnly) {
                AddRecipeItems(items, query);
            }

            if ((bool)Properties.Settings.Default.ShowAugmentsAsItems && !query.SocketedOnly) {
                AddAugmentItems(items, query);
            }

            

            _itemPaginatorService.Update(items, orderByLevel);
            

            return message;
        }

        private void AddBuddyItems(List<PlayerHeldItem> items, Search query, out string message) {
            List<BuddyItem> buddyItems = new List<BuddyItem>(_buddyItemDao.FindBy(query));
            List<PlayerHeldItem> itemsWithBuddy = items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord));

            foreach (PlayerHeldItem item in items.FindAll(item => buddyItems.Any(buddy => buddy.BaseRecord == item.BaseRecord))) {
                foreach (PlayerHeldItem buddyItem in buddyItems.FindAll(buddy => buddy.BaseRecord == item.BaseRecord)) {
                        var buddyName = System.Web.HttpUtility.HtmlEncode(buddyItem.Stash);
                        if(!item.Buddies.Exists(name => name == buddyName))
                            item.Buddies.Add(buddyName);
                }
            }
            _logger.Debug($"Merged {itemsWithBuddy.Count} buddy items into player items");


            // TODO: This should use .Except(), find out why its not working with .Except()
            List<BuddyItem> remainingBuddyItems = buddyItems.FindAll(buddy => !itemsWithBuddy.Any(item => item.BaseRecord == buddy.BaseRecord));
            
            //We add the Owner name from pure BuddyItems from Stash to BuddyNames
            foreach (PlayerHeldItem remainingBuddyItem in remainingBuddyItems) {
                var buddyName = System.Web.HttpUtility.HtmlEncode(remainingBuddyItem.Stash);
                remainingBuddyItem.Buddies.Add(buddyName);
            }

            //We see if of the remaining Items there are any items with more than one Buddy and merge them
            List<BuddyItem> multiBuddyItems = remainingBuddyItems.FindAll(item => remainingBuddyItems.FindAll(buddy => buddy.BaseRecord == item.BaseRecord).Count() > 1);
            foreach (PlayerHeldItem multiBuddyItem in multiBuddyItems) {
                foreach (PlayerHeldItem item in remainingBuddyItems.FindAll(item => multiBuddyItem.BaseRecord == item.BaseRecord)) {
                    var buddyName = System.Web.HttpUtility.HtmlEncode(item.Stash);
                    if (!multiBuddyItem.Buddies.Exists(name => name == buddyName))
                        multiBuddyItem.Buddies.Add(buddyName);
                }
            }

            var buddyPlayerHeldItems = new List<PlayerHeldItem>(remainingBuddyItems);
            if (buddyPlayerHeldItems.Count > 0) {
                MergeDuplicates(buddyPlayerHeldItems);
                items.AddRange(buddyPlayerHeldItems);
                message = $"An additional {buddyPlayerHeldItems.Count} items found from your friends";
            }
            else {
                message = string.Empty;
            }
        }

        private void AddAugmentItems(List<PlayerHeldItem> items, Search query) {
            var augments = _augmentationItemRepo.Search(query);
            var remainingRecipes = augments.Where(recipe => items.All(item => item.BaseRecord != recipe.BaseRecord));
            items.AddRange(remainingRecipes);
        }

        private void AddRecipeItems(List<PlayerHeldItem> items, Search query) {
            var recipes = _dbItemDao.SearchForRecipeItems(query);

            List<PlayerHeldItem> itemsWithRecipe = items.FindAll(item => recipes.Any(recipe => recipe.BaseRecord == item.BaseRecord));
            foreach (PlayerHeldItem item in items) {
                if (itemsWithRecipe.Any(recipe => recipe.BaseRecord == item.BaseRecord)) {
                    item.HasRecipe = true;
                }
            }

            // TODO: This should use .Except(), find out why its not working with .Except()
            var remainingRecipes = recipes.Where(recipe => !itemsWithRecipe.Any(item => item.BaseRecord == recipe.BaseRecord));
            items.AddRange(remainingRecipes);
        }

        /// <summary>
        /// Merge player held items
        /// IFF MergeDuplicates is enabled in the settings
        /// </summary>
        /// <param name="items"></param>
        private void MergeDuplicates(List<PlayerHeldItem> items) {
            if ((bool)Properties.Settings.Default.MergeDuplicates) {
                Dictionary<string, PlayerHeldItem> itemMap = new Dictionary<string, PlayerHeldItem>();

                foreach (PlayerHeldItem item in items) {
                    if (!item.IsKnown)
                        continue;

                    var key = item.Name + item.Stash;

                    if (itemMap.ContainsKey(key)) {
                        itemMap[key].Count += item.Count;
                    } else {
                        itemMap[key] = item;
                    }
                }

                items.RemoveAll(m => m.IsKnown);
                items.AddRange(itemMap.Values);
            }
        }

    }
}
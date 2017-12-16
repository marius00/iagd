using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Services.Crafting.dto;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation.PredefinedTransformations;

namespace IAGrim.Services.Crafting {
    class CostCalculationService {
        private readonly IPlayerItemDao _playerItemDao;
        private readonly StashManager _stashManager;
        private string _mod;

        public CostCalculationService(IPlayerItemDao playerItemDao, StashManager stashManager) {
            _playerItemDao = playerItemDao;
            _stashManager = stashManager;
        }

        public void SetMod(string mod) {
            _mod = mod;
        }

        public void Populate(ComponentCost recipe) {
            if (recipe != null) {
                ISet<string> recipeRecords = new HashSet<string>();
                GetRecords(recipe, recipeRecords);

                // Fetch player held items
                Dictionary<string, int> storedItems = _playerItemDao.GetCountByRecord(_mod);
                var stashItems = _stashManager.UnlootedItems
                    .Where(item => recipeRecords.Contains(item.BaseRecord))
                    .Where(item => item.MateriaCombines == 0 || item.MateriaCombines >= 3) // "Good enough" for now, TODO: Check the DB for the real amount
                    .ToList();

                // Merge the two lists
                foreach (var item in stashItems) {
                    if (storedItems.ContainsKey(item.BaseRecord)) {
                        storedItems[item.BaseRecord] += Math.Max(1, (int) item.StackCount);
                    }
                    else {
                        storedItems[item.BaseRecord] = Math.Max(1, (int) item.StackCount);
                    }
                }

                // Apply "already have" items
                for (int depth = 0; depth < 10; depth++) {
                    ApplyIngredientsDepthWise(recipe, storedItems, depth);
                }
            }
        }

        private static void ApplyIngredientsDepthWise(ComponentCost recipe, Dictionary<string, int> owned, int remainingDepth) {
            if (remainingDepth == 0) {
                if (owned.ContainsKey(recipe.Record)) {
                    var numSpent = Math.Min(owned[recipe.Record], recipe.NumRequired);
                    owned[recipe.Record] -= numSpent;
                    recipe.NumOwned = numSpent;

                    if (numSpent > 0) {
                        ReduceChildrenTotals(recipe, recipe.NumRequired, recipe.NumRequired - numSpent);
                    }
                }
            }
            else if (remainingDepth > 0) {
                if (recipe.NumOwned >= recipe.NumRequired) {
                    MarkComplete(recipe);
                }
                else {
                    foreach (var child in recipe.Cost) {
                        ApplyIngredientsDepthWise(child, owned, remainingDepth - 1);
                    }
                }
            }
        }

        private static void ReduceChildrenTotals(ComponentCost recipe, int original, int newValue) {
            foreach (var child in recipe.Cost) {
                child.NumRequired = child.NumRequired / original * newValue;
                ReduceChildrenTotals(child, original, newValue);
            }
        }

        private static void MarkComplete(ComponentCost recipe) {
            recipe.IsComplete = true;
            foreach (var child in recipe.Cost) {
                MarkComplete(child);
            }
        }

        private static void GetRecords(ComponentCost recipe, ISet<string> records) {
            records.Add(recipe.Record);
            foreach (var cost in recipe.Cost) {
                GetRecords(cost, records);
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Services.Crafting.dto;
using log4net;

namespace IAGrim.Services.Crafting {
    class RecipeService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecipeService));
        private readonly IDatabaseItemDao _itemDao;
        private readonly string[] _reagents = { "reagentBase", "reagent1", "reagent2", "reagent3", "reagent4", "reagent5", "reagent6" };
        private readonly List<DatabaseItemDto> _formulas;
        private readonly List<DatabaseItemDto> _craftables;


        // These components form a loop, break the loop when one is spotted [Stack Overflow]
        private readonly string[] _looped = {
            "records/items/crafting/materials/craft_bloodchthon.dbr",
            "records/items/crafting/materials/craft_taintedbrain.dbr",
            "records/items/crafting/materials/craft_ancientheart.dbr",
            "records/items/materia/compa_aethercrystal.dbr",
            "records/items/materia/compa_crackedlodestone.dbr",
            "records/items/materia/compa_searingember.dbr",
            "records/items/materia/compa_polishedemerald.dbr",
            "records/items/materia/compa_chilledsteel.dbr",
            "records/items/materia/compa_scavengedplating.dbr",
            "records/items/crafting/materials/craft_wendigospirit.dbr",
            "records/items/crafting/materials/craft_aetherialmissive.dbr",
            "records/items/crafting/materials/craft_aetherialmutagen.dbr"
        };


        public RecipeService(IDatabaseItemDao itemDao) {
            this._itemDao = itemDao;
            _formulas = _itemDao.GetByClass("ItemArtifactFormula");
            _craftables = _itemDao.GetCraftableItems();
        }


        public CraftableRecipes GetRecipeList() {
            var relics = _craftables.Where(m => m.Record.Contains("relic"))
                .Select(m => new ComponentListEntry { Label = m.Name, Record = m.Record });

            var components = _craftables.Where(m => m.Stats.Any(s => s.Stat == "Class" && s.TextValue == "ItemRelic") && !string.IsNullOrEmpty(m.Name))
                .Select(m => new ComponentListEntry { Label = m.Name, Record = m.Record }).ToList();

            var misc = _craftables.Where(m => !m.Record.Contains("relic") && !string.IsNullOrEmpty(m.Name))
                .Where(m => !m.Stats.Any(s => s.Stat == "Class" && s.TextValue == "ItemRelic"))
                .Select(m => new ComponentListEntry { Label = m.Name, Record = m.Record });

            return new CraftableRecipes {
                Relics = relics.ToList(),
                Misc = misc.ToList(),
                Components = components
            };
        }


        /// <summary>
        /// Get the ingredients / cost of a given recipe
        /// </summary>
        /// <param name="recordId">Base record of the recipe</param>
        /// <returns></returns>
        public ComponentCost GetRecipeIngredients(string recordId) {

            if (_craftables.Any(m => m.Record == recordId)) {
                var comp = CreateComponent(_formulas, _craftables, recordId, 1);
                return comp;
            }

            Logger.Warn($"Attempted to retrieve ingredients for unknown recipe {recordId}");
            return null;
        }

        /// <summary>
        /// Get the reference to this items bitmap
        /// </summary>
        private static string GetBitmap(ISet<RecipeDbStatRow> stats) {
            string result = string.Empty;
            if (stats != null) {
                if (stats.Any(m => "bitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "bitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => "relicBitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "relicBitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => "shardBitmap".Equals(m.Stat)))
                    result = stats.FirstOrDefault(m => "shardBitmap".Equals(m.Stat))?.TextValue;

                else if (stats.Any(m => m.Stat.Contains("itmap")))
                    result = stats.FirstOrDefault(m => m.Stat.Contains("itmap"))?.TextValue;

            }

            return $"{Path.GetFileName(result?.Replace(".dbr", ".tex"))}.png";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formulas">Every formula in the database</param>
        /// <param name="components">Every component craftable by a formula</param>
        /// <param name="record"></param>
        /// <param name="numRequired"></param>
        /// <param name="depth">Max search depth to prevent stack overflows etc</param>
        /// <returns>A tree showing the construction costs for the given record</returns>
        private ComponentCost CreateComponent(IList<DatabaseItemDto> formulas, IList<DatabaseItemDto> components, string record, int numRequired, int depth = 0) {
            // Obs: if 'item' is null, search the database for it, some items are components without being craftable
            var item = components.FirstOrDefault(m => m.Record == record);
            if (item == null) {
                item = _itemDao.FindDtoByRecord(record);
                Logger.Warn($"Component {record} was not preloaded, this may be a performance issue");
                components.Add(item);
            }
            
            
            var result = new ComponentCost {
                NumRequired = numRequired,
                Name = item?.Name,
                Bitmap = GetBitmap(item?.Stats),
                Cost = new List<ComponentCost>(),
                Record = record
            };

            if (_looped.Contains(record) || depth >= 10)
                return result;



            var recipeStat = formulas.SelectMany(m => m.Stats).FirstOrDefault(m => m.Stat == "artifactName" && m.TextValue == record);


            // For stuff like claws we only crate 1/4, so need the real number required
            // Only do this if this is not the last item in the chain. (makes no sense to list 4x when you obviously need a complete)
            int costMultiplier = numRequired;
            var relicLevel = item?.Stats.FirstOrDefault(m => m.Stat == "completedRelicLevel");
            bool nonPartialCraft = recipeStat?.Parent.Stats.Any(m => m.Stat == "forcedRelicCompletion") ?? false;
            if (relicLevel != null && !nonPartialCraft) {
                costMultiplier *= (int)relicLevel.Value;
            }

            // Calculate costs for this item/component
            var recipe = recipeStat?.Parent;
            foreach (var reagent in _reagents) {
                var rcd = (recipe?.Stats)?.FirstOrDefault(m => m.Stat == $"{reagent}BaseName")?.TextValue;
                var qtd = (recipe?.Stats)?.FirstOrDefault(m => m.Stat == $"{reagent}Quantity")?.Value;
                if (rcd != null && qtd.HasValue) {
                    result.Cost.Add(CreateComponent(formulas, components, rcd, (int)qtd.Value * costMultiplier, depth + 1));
                }
            }




            return result;
        }
    }
}

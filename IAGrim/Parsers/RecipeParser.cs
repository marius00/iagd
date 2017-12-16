using IAGrim.Database;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IAGrim.Utilities;
using NHibernate.Criterion;
using EvilsoftCommons.Exceptions;
using EvilsoftCommons;
using IAGrim.Database.Interfaces;

namespace IAGrim.Parsers {
    public class RecipeParser {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecipeParser));
        private readonly IRecipeItemDao _recipeItemDao;

        public RecipeParser(IRecipeItemDao recipeItemDao) {
            this._recipeItemDao = recipeItemDao;
        }

        public class FormulaEntry {
            public SortedSet<string> Formulas;
            public bool IsExpansion1;
        }

        /// <summary>
        /// Parse a Grim Dawn recipe file (userdata)
        /// </summary>
        /// <param name="formulasGstFile"></param>
        /// <returns>List of recipe paths or null if an invalid format</returns>
        private static FormulaEntry ReadFormulas(string formulasGstFile) {
            SortedSet<string> result = new SortedSet<string>();
            bool isExpansion1 = false;

            using (FileStream fs = new FileStream(formulasGstFile, FileMode.Open)) {

                if (!"begin_block".Equals(IOHelper.ReadString(fs))) return null;
                uint unknown0 = IOHelper.ReadUInteger(fs);

                if (!"formulasVersion".Equals(IOHelper.ReadString(fs))) return null;
                uint version = IOHelper.ReadUInteger(fs);

                if (!"numEntries".Equals(IOHelper.ReadString(fs))) return null;
                uint numItems = IOHelper.ReadUInteger(fs);

                //uint unknown2 = IOHelper.ReadUInteger(fs);

                if (version >= 3) {
                    if (!"expansionStatus".Equals(IOHelper.ReadString(fs)))
                        return null;

                    isExpansion1 = fs.ReadByte() != 0;
                }


                for (uint i = 0; i < numItems; i++) {
                    if (!"itemName".Equals(IOHelper.ReadString(fs)))
                        return null;

                    result.Add(IOHelper.ReadString(fs));

                    if (!"formulaRead".Equals(IOHelper.ReadString(fs)))
                        return null;

                    uint unknown1 = IOHelper.ReadUInteger(fs);
                }

                if (!"end_block".Equals(IOHelper.ReadString(fs)))
                    return null;
            }

            // May not have been included in the recipe file, always craftable
            const string relicCalamity = "records/items/crafting/blueprints/relic/craft_relic_b001.dbr";
            const string relicRuination = "records/items/crafting/blueprints/relic/craft_relic_b002.dbr";
            const string relicEquilibrium = "records/items/crafting/blueprints/relic/craft_relic_b003.dbr";
            result.Add(relicCalamity);
            result.Add(relicRuination);
            result.Add(relicEquilibrium);

            return new FormulaEntry {
                Formulas = result,
                IsExpansion1 = isExpansion1
            };            
        }

        /// <summary>
        /// Update the internal database with player formulas
        /// </summary>
        /// <param name="formulasGstFile">Path to player formula file, generally my games\grim dawn</param>
        public void UpdateFormulas(string formulasGstFile, bool isHardcore) {            
            try {
                FormulaEntry formulaEntries = ReadFormulas(formulasGstFile);
                ICollection<string> formulas = formulaEntries.Formulas;
                if (formulas == null) {
                    Logger.Error("Error reading recipe file, formula file either corrupted or file format has changed");
                    return;
                }

                formulas = formulas.Where(formula => !formula.StartsWith("records/items/crafting/blueprints/component")
                                    && !formula.StartsWith("records/items/crafting/blueprints/other/craft_elixir")
                                    && !formula.StartsWith("records/items/crafting/blueprints/other/craft_oil")
                                    && !formula.StartsWith("records/items/crafting/blueprints/other/craft_potion")
                                    && !formula.StartsWith("records/items/crafting/blueprints/other/craft_special")
                                    ).ToList();



                ISet<string> alreadyProcessed = new SortedSet<string>();
                if (formulas.Count > 0) {
                    _recipeItemDao.DeleteAll(isHardcore);

                    // References to the actual item [eg artifactName => record]
                    IList<DatabaseItemStat> stats = _recipeItemDao.GetRecipeStats(formulas);

                    // Stats for the actual item
                    IList<DatabaseItemStat> itemStats = _recipeItemDao.GetRecipeItemStats(
                        stats.Select(m => m.TextValue).ToArray()
                    );

                    List<RecipeItem> toSave = new List<RecipeItem>();
                    foreach (string formula in formulas) {
                        var q = stats.Where(s => s.Parent.Record.Equals(formula));
                        if (q.Any()) {
                            DatabaseItemStat item = q.First();
                            if (!string.IsNullOrEmpty(item.TextValue)) {
                                var minimumLevel = itemStats
                                    .Where(m => m.Parent.Record.Equals(item.TextValue))
                                    .Where(m => "levelRequirement".Equals(m.Stat))
                                    .FirstOrDefault()?.Value ?? 0f;


                                if (!alreadyProcessed.Contains(item.TextValue)) {
                                    toSave.Add(new RecipeItem {
                                        BaseRecord = item.TextValue,
                                        IsHardcore = isHardcore,
                                        MinimumLevel = minimumLevel,
                                        IsExpansion1 = formulaEntries.IsExpansion1

                                    });

                                    alreadyProcessed.Add(item.TextValue);
                                }
                            }
                            else {
                                Logger.Warn($"Could not parse formula \"{formula}\"");
                            }
                        }
                    }

                    _recipeItemDao.Save(toSave);                        
                    Logger.InfoFormat("Updated internal recipe database with {0} recipes.", formulas.Count);
                }
            }
            catch (NHibernate.MappingException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                    
                ExceptionReporter.ReportException(ex, "UpdateFormulas");
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "UpdateFormulas");
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Parsers.GameDataParsing.UI;

namespace IAGrim.Parsers.GameDataParsing.Service {
    public class ParsingService {
        private string _grimdawnLocation;
        private string _modLocation;

        private readonly IItemTagDao _itemTagDao;
        private readonly IDatabaseItemDao _databaseItemDao;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IItemSkillDao _itemSkillDao;
        private readonly string _localizationFile;


        public ParsingService(
            IItemTagDao itemTagDao, 
            string grimdawnLocation, 
            IDatabaseItemDao databaseItemDao, 
            IDatabaseItemStatDao databaseItemStatDao, 
            IItemSkillDao itemSkillDao, 
            string localizationFile
        ) {
            _itemTagDao = itemTagDao;
            _grimdawnLocation = grimdawnLocation;
            _databaseItemDao = databaseItemDao;
            _databaseItemStatDao = databaseItemStatDao;
            _itemSkillDao = itemSkillDao;
            _localizationFile = localizationFile;
        }

        public void Update(string install, string mod) {
            _grimdawnLocation = install;
            _modLocation = mod;
        }

        public void Execute() {
            var form = new ParsingDatabaseProgressView();
            var parser = new ArzParsingWrapper();

            
            string vanillaTags = GrimFolderUtility.FindArcFile(_grimdawnLocation, "text_en.arc");
            string expansion1Tags = GrimFolderUtility.FindArcFile(Path.Combine(_grimdawnLocation, "gdx1"), "text_en.arc");
            string modTags = string.IsNullOrEmpty(_modLocation) ? "" : GrimFolderUtility.FindArcFile(_modLocation, "text_en.arc");

            List<Action> actions = new List<Action>();
            actions.Add(() => parser.LoadTags(vanillaTags, expansion1Tags, modTags, _localizationFile, new WinformsProgressBar(form.LoadingTags).Tracker));
            actions.Add(() => _itemTagDao.Save(parser.Tags, new WinformsProgressBar(form.SavingTags).Tracker));

            string vanillaItems = GrimFolderUtility.FindArzFile(_grimdawnLocation);
            string expansion1Items = GrimFolderUtility.FindArzFile(Path.Combine(_grimdawnLocation, "gdx1"));
            string modItems = string.IsNullOrEmpty(_modLocation) ? "" : GrimFolderUtility.FindArzFile(_modLocation);
            actions.Add(() => parser.LoadItems(vanillaItems, expansion1Items, modItems, new WinformsProgressBar(form.LoadingItems).Tracker));

            // TODO: 
            actions.Add(() => parser.MapItemNames(new WinformsProgressBar(form.MappingItemNames).Tracker));
            actions.Add(() => parser.RenamePetStats(new WinformsProgressBar(form.MappingPetStats).Tracker));

            actions.Add(() => _databaseItemDao.Save(parser.Items, new WinformsProgressBar(form.SavingItems).Tracker));

            // TODO: This depends on the DB item name.. which is in english, not localized
            actions.Add(() => {
                var records = parser.GenerateSpecialRecords(new WinformsProgressBar(form.GeneratingSpecialStats).Tracker);
                _databaseItemStatDao.Save(records, new WinformsProgressBar(form.SavingSpecialStats).Tracker);
            });


            actions.Add(() => parser.ParseComplexItems(_itemSkillDao, new WinformsProgressBar(form.GeneratingSkills).Tracker));
            actions.Add(() => {
                var tracker = new WinformsProgressBar(form.SkillCorrectnessCheck).Tracker;
                tracker.MaxValue = 1;
                _itemSkillDao.EnsureCorrectSkillRecords();
                tracker.MaxProgress();
            });

            // Invoke the background thread & show progress UI
            Thread t = new Thread(() => {
                foreach (var action in actions) {
                    action.Invoke();
                }

                Action close = () => form.OverrideClose();
                form.Invoke(close);
            });

            t.Start();
            form.ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Parsers.GameDataParsing.UI;
using log4net;

namespace IAGrim.Parsers.GameDataParsing.Service {
    public class ParsingService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ParsingService));
        private string _grimdawnLocation;
        private string? _modLocation;

        private readonly IItemTagDao _itemTagDao;
        private readonly IDatabaseItemDao _databaseItemDao;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IItemSkillDao _itemSkillDao;
        private readonly string _languageCode;
        public event EventHandler? OnParseComplete;


        public ParsingService(
            IItemTagDao itemTagDao,
            string grimdawnLocation,
            IDatabaseItemDao databaseItemDao,
            IDatabaseItemStatDao databaseItemStatDao,
            IItemSkillDao itemSkillDao,
            string languageCode
        ) {
            _itemTagDao = itemTagDao;
            _grimdawnLocation = grimdawnLocation;
            _databaseItemDao = databaseItemDao;
            _databaseItemStatDao = databaseItemStatDao;
            _itemSkillDao = itemSkillDao;
            _languageCode = languageCode;
        }

        public static long GetHighestTimestamp(string install) {
            try {
                List<string> arzFiles = new List<string> {
                    GrimFolderUtility.FindArzFile(install)
                };

                foreach (string path in GrimFolderUtility.GetGrimExpansionFolders(install)) {
                    string expansionItems = GrimFolderUtility.FindArzFile(path);

                    if (!string.IsNullOrEmpty(expansionItems)) {
                        arzFiles.Add(GrimFolderUtility.FindArzFile(expansionItems));
                    }
                }

                return arzFiles
                    .Select(File.GetLastWriteTimeUtc)
                    .Select(ts => ts.ToTimestamp())
                    .Max();
            }
            catch (Exception e) {
                Logger.Warn("Error fetching timestamp, defaulting to unchanged", e);
                return 0;
            }
        }

        public void Update(string install, string mod) {
            _grimdawnLocation = install;
            _modLocation = mod;
        }

        public void Execute() {
            var form = new ParsingDatabaseProgressView();
            var parser = new ArzParsingWrapper();

            string arcFileName = $"text_{_languageCode.ToLowerInvariant()}.arc";

            // Always load English first as fallback, then overlay selected language
            List<string> tagfiles = new List<string>();

            // English tags first (fallback)
            string vanillaEnTags = GrimFolderUtility.FindArcFile(_grimdawnLocation, "text_en.arc");
            if (!string.IsNullOrEmpty(vanillaEnTags)) {
                tagfiles.Add(vanillaEnTags);
            }

            foreach (string path in GrimFolderUtility.GetGrimExpansionFolders(_grimdawnLocation)) {
                string expansionEnTags = GrimFolderUtility.FindArcFile(path, "text_en.arc");
                if (!string.IsNullOrEmpty(expansionEnTags)) {
                    tagfiles.Add(expansionEnTags);
                }
            }

            string modEnTags = string.IsNullOrEmpty(_modLocation) ? "" : GrimFolderUtility.FindArcFile(_modLocation, "text_en.arc");
            if (!string.IsNullOrEmpty(modEnTags)) {
                tagfiles.Add(modEnTags);
            }

            // Selected language overlay (if not English)
            if (!_languageCode.Equals("EN", StringComparison.OrdinalIgnoreCase)) {
                string vanillaLangTags = GrimFolderUtility.FindArcFile(_grimdawnLocation, arcFileName);
                if (!string.IsNullOrEmpty(vanillaLangTags)) {
                    tagfiles.Add(vanillaLangTags);
                }

                foreach (string path in GrimFolderUtility.GetGrimExpansionFolders(_grimdawnLocation)) {
                    string expansionLangTags = GrimFolderUtility.FindArcFile(path, arcFileName);
                    if (!string.IsNullOrEmpty(expansionLangTags)) {
                        tagfiles.Add(expansionLangTags);
                    }
                }

                string modLangTags = string.IsNullOrEmpty(_modLocation) ? "" : GrimFolderUtility.FindArcFile(_modLocation, arcFileName);
                if (!string.IsNullOrEmpty(modLangTags)) {
                    tagfiles.Add(modLangTags);
                }
            }




            List<string> arzFiles = new List<string> {
                GrimFolderUtility.FindArzFile(_grimdawnLocation)
            };

            foreach (string path in GrimFolderUtility.GetGrimExpansionFolders(_grimdawnLocation)) {
                string expansionItems = GrimFolderUtility.FindArzFile(path);

                if (!string.IsNullOrEmpty(expansionItems)) {
                    arzFiles.Add(GrimFolderUtility.FindArzFile(expansionItems));
                }
            }

            if (!string.IsNullOrEmpty(_modLocation)) {
                arzFiles.Add(GrimFolderUtility.FindArzFile(_modLocation));
            }


            // Invoke the background thread & show progress UI
            Thread t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();
                parser.LoadTags(tagfiles, _languageCode, new WinformsProgressBar(form.LoadingTags).Tracker);
                _itemTagDao.Save(parser.Tags, new WinformsProgressBar(form.SavingTags).Tracker);
                parser.LoadItems(arzFiles, new WinformsProgressBar(form.LoadingItems).Tracker);
                parser.MapItemNames(new WinformsProgressBar(form.MappingItemNames).Tracker);
                parser.RenamePetStats(new WinformsProgressBar(form.MappingPetStats).Tracker);
                _databaseItemDao.Save(parser.Items ?? [], new WinformsProgressBar(form.SavingItems).Tracker);
                _databaseItemDao.CreateItemIndexes(new WinformsProgressBar(form.IndexingItems).Tracker);

                // TODO: This depends on the DB item name.. which is in english, not localized
                {
                    var records = parser.GenerateSpecialRecords(new WinformsProgressBar(form.GeneratingSpecialStats).Tracker);
                    _databaseItemStatDao.Save(records, new WinformsProgressBar(form.SavingSpecialStats).Tracker);
                };


                parser.ParseComplexItems(_itemSkillDao, new WinformsProgressBar(form.GeneratingSkills).Tracker);
                {
                    var tracker = new WinformsProgressBar(form.SkillCorrectnessCheck).Tracker;
                    tracker.MaxValue = 1;
                    _itemSkillDao.EnsureCorrectSkillRecords();
                    tracker.MaxProgress();
                };


                Action close = () => form.OverrideClose();
                form.Invoke(close);
            });

            t.Start();
            form.ShowDialog();

            OnParseComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}
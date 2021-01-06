using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Migrations;
using IAGrim.Database.Synchronizer;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Parsers;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Parsers.TransferStash;
using IAGrim.Settings;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    public class ServiceProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceProvider));
        private readonly List<object> _services;
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        private ServiceProvider(List<object> services) {
            _services = services;
        }

        public T Get<T>() {
            var typeOf = typeof(T);
            if (_cache.ContainsKey(typeOf)) {
                return (T)_cache[typeOf];
            }

            foreach (var service in _services) {
                if (service.GetType() == typeOf || service.GetType().GetInterfaces().Contains(typeOf)) {
                    _cache[typeOf] = service;
                    return (T) service;
                }
            }

            throw new ArgumentException("Cannot find service of type", typeof(T).ToString());
        }

        public void Add(object o) {
            _services.Add(o);
        }

        public static ServiceProvider Initialize(ThreadExecuter threadExecuter, SqlDialect dialect) {
            Logger.Debug("Creating services");
            var factory = new SessionFactory(dialect);

            // Settings should be upgraded early, it contains the language pack etc and some services depends on settings.
            var settingsService = StartupService.LoadSettingsService();
            var grimDawnDetector = new GrimDawnDetector(settingsService);

            IPlayerItemDao playerItemDao;
            IDatabaseItemDao databaseItemDao;
            IDatabaseSettingDao databaseSettingDao;
            IDatabaseItemStatDao databaseItemStatDao;
            IItemTagDao itemTagDao;
            IBuddyItemDao buddyItemDao;
            IBuddySubscriptionDao buddySubscriptionDao;
            IRecipeItemDao recipeItemDao;
            IItemSkillDao itemSkillDao;
            IAugmentationItemDao augmentationItemRepo;
            IItemCollectionDao itemCollectionRepo;

            if (dialect == SqlDialect.Sqlite) {
                playerItemDao = new PlayerItemRepo(threadExecuter, factory, dialect);
                databaseItemDao = new DatabaseItemRepo(threadExecuter, factory, dialect);
                databaseSettingDao = new DatabaseSettingRepo(threadExecuter, factory, dialect);
                databaseItemStatDao = new DatabaseItemStatRepo(threadExecuter, factory, dialect);
                itemTagDao = new ItemTagRepo(threadExecuter, factory, dialect);
                buddyItemDao = new BuddyItemRepo(threadExecuter, factory, dialect);
                buddySubscriptionDao = new BuddySubscriptionRepo(threadExecuter, factory, dialect);
                recipeItemDao = new RecipeItemRepo(threadExecuter, factory, dialect);
                itemSkillDao = new ItemSkillRepo(threadExecuter, factory);
                augmentationItemRepo = new AugmentationItemRepo(threadExecuter, factory, new DatabaseItemStatDaoImpl(factory, dialect), dialect);
                itemCollectionRepo = new ItemCollectionRepo(threadExecuter, factory, dialect);
            }
            else {
                databaseItemStatDao = new DatabaseItemStatDaoImpl(factory, dialect);
                playerItemDao = new PlayerItemDaoImpl(factory, databaseItemStatDao, dialect);
                databaseItemDao = new DatabaseItemDaoImpl(factory, dialect);
                databaseSettingDao = new DatabaseSettingDaoImpl(factory, dialect);
                itemTagDao = new ItemTagDaoImpl(factory, dialect);
                buddyItemDao = new BuddyItemDaoImpl(factory, dialect);
                buddySubscriptionDao = new BuddySubscriptionDaoImpl(factory, dialect);
                recipeItemDao = new RecipeItemDaoImpl(factory, dialect);
                itemSkillDao = new ItemSkillDaoImpl(factory);
                augmentationItemRepo = new AugmentationItemDaoImpl(factory, databaseItemStatDao, dialect);
                itemCollectionRepo = new ItemCollectionDaoImpl(factory, dialect);
            }

            // Chicken and the egg..
            var itemStatService = new ItemStatService(databaseItemStatDao, itemSkillDao, settingsService);
            SearchController searchController = new SearchController(
                databaseItemDao,
                playerItemDao,
                itemStatService,
                buddyItemDao,
                augmentationItemRepo,
                settingsService,
                itemCollectionRepo
            );

            List<object> services = new List<object>();
            services.Add(itemTagDao);
            services.Add(databaseItemDao);
            services.Add(databaseItemStatDao);
            services.Add(playerItemDao);
            services.Add(databaseSettingDao);
            services.Add(buddyItemDao);
            services.Add(buddySubscriptionDao);
            services.Add(itemSkillDao);
            services.Add(augmentationItemRepo);
            //services.Add(userFeedbackService);
            services.Add(settingsService);
            services.Add(grimDawnDetector);
            services.Add(recipeItemDao);
            services.Add(itemCollectionRepo);
            services.Add(searchController);

            services.Add(itemStatService);

            var cacher = new TransferStashServiceCache(databaseItemDao);
            var stashWriter = new SafeTransferStashWriter(settingsService);
            var transferStashService = new TransferStashService(databaseItemStatDao, settingsService, stashWriter);
            services.Add(transferStashService);
            services.Add(cacher);

            Logger.Debug("All services created");
            return new ServiceProvider(services);
        }
    }
}
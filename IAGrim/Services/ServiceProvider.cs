using System;
using System.Collections.Generic;
using System.Linq;
using IAGrim.Database;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Synchronizer;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Parsers.TransferStash;
using IAGrim.Services.ItemReplica;
using IAGrim.Services.MessageProcessor;
using IAGrim.UI.Controller;
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
            IDatabaseItemStatDao databaseItemStatDao;
            IItemTagDao itemTagDao;
            IBuddyItemDao buddyItemDao;
            IBuddySubscriptionDao buddySubscriptionDao;
            IItemSkillDao itemSkillDao;
            IItemCollectionDao itemCollectionRepo;
            IReplicaItemDao replicaItemDao;

            if (dialect == SqlDialect.Sqlite) {
                playerItemDao = new PlayerItemRepo(threadExecuter, factory, dialect);
                databaseItemDao = new DatabaseItemRepo(threadExecuter, factory, dialect);
                databaseItemStatDao = new DatabaseItemStatRepo(threadExecuter, factory, dialect);
                itemTagDao = new ItemTagRepo(threadExecuter, factory, dialect);
                buddyItemDao = new BuddyItemRepo(threadExecuter, factory, dialect);
                buddySubscriptionDao = new BuddySubscriptionRepo(threadExecuter, factory, dialect);
                itemSkillDao = new ItemSkillRepo(threadExecuter, factory);
                itemCollectionRepo = new ItemCollectionRepo(threadExecuter, factory, dialect);
                replicaItemDao = new ItemReplicaRepo(threadExecuter, factory, dialect);
            }
            else {
                databaseItemStatDao = new DatabaseItemStatDaoImpl(factory, dialect);
                playerItemDao = new PlayerItemDaoImpl(factory, databaseItemStatDao, dialect);
                databaseItemDao = new DatabaseItemDaoImpl(factory, dialect);
                itemTagDao = new ItemTagDaoImpl(factory, dialect);
                buddyItemDao = new BuddyItemDaoImpl(factory, databaseItemStatDao, dialect);
                buddySubscriptionDao = new BuddySubscriptionDaoImpl(factory, dialect);
                itemSkillDao = new ItemSkillDaoImpl(factory);
                itemCollectionRepo = new ItemCollectionDaoImpl(factory, dialect);
                replicaItemDao = new ReplicaItemDaoImpl(factory, dialect);
            }
            

            // Chicken and the egg..
            var itemStatService = new ItemStatService(databaseItemStatDao, itemSkillDao, settingsService);
            SearchController searchController = new SearchController(
                playerItemDao,
                itemStatService,
                buddyItemDao,
                itemCollectionRepo
            );

            List<object> services = new List<object>();
            services.Add(itemTagDao);
            services.Add(databaseItemDao);
            services.Add(databaseItemStatDao);
            services.Add(playerItemDao);
            services.Add(buddyItemDao);
            services.Add(buddySubscriptionDao);
            services.Add(itemSkillDao);
            
            services.Add(settingsService);
            services.Add(grimDawnDetector);
            services.Add(itemCollectionRepo);
            services.Add(searchController);
            services.Add(new ItemReplicaService(playerItemDao, buddyItemDao, settingsService));
            services.Add(replicaItemDao);

            services.Add(itemStatService);

            var cacher = new TransferStashServiceCache(databaseItemDao);
            services.Add(cacher);


            Logger.Debug("All services created");
            return new ServiceProvider(services);
        }
    }
}
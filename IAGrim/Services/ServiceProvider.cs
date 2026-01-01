using IAGrim.Database;
using IAGrim.Database.DAO;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.TransferStash;
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

        public static ServiceProvider Initialize() {
            Logger.Debug("Creating services");
            var factory = new SessionFactory();

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

            databaseItemStatDao = new DatabaseItemStatDaoImpl(factory);
            playerItemDao = new PlayerItemDaoImpl(factory, databaseItemStatDao);
            databaseItemDao = new DatabaseItemDaoImpl(factory);
            itemTagDao = new ItemTagDaoImpl(factory);
            buddyItemDao = new BuddyItemDaoImpl(factory, databaseItemStatDao);
            buddySubscriptionDao = new BuddySubscriptionDaoImpl(factory);
            itemSkillDao = new ItemSkillDaoImpl(factory);
            itemCollectionRepo = new ItemCollectionDaoImpl(factory);
            replicaItemDao = new ReplicaItemDaoImpl(factory);

            

            // Chicken and the egg..
            var itemStatService = new ItemStatService(databaseItemStatDao, itemSkillDao, settingsService);
            SearchController searchController = new SearchController(
                playerItemDao,
                itemStatService,
                buddyItemDao,
                itemCollectionRepo
            );

            List<object> services = [
                itemTagDao,
                databaseItemDao,
                databaseItemStatDao,
                playerItemDao,
                buddyItemDao,
                buddySubscriptionDao,
                itemSkillDao,
                settingsService,
                grimDawnDetector,
                itemCollectionRepo,
                searchController,
                new ItemReplicaService(playerItemDao, buddyItemDao, settingsService),
                replicaItemDao,
                itemStatService
            ];

            var cacher = new TransferStashServiceCache(databaseItemDao);
            services.Add(cacher);


            Logger.Debug("All services created");
            return new ServiceProvider(services);
        }
    }
}
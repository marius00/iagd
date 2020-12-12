using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using EvilsoftCommons;
using IAGrim.Utilities;

namespace SessionFactoryLoader {
    public class SessionFactory : ISessionFactoryWrapper {
        private ISessionFactory _sessionFactory;
        private readonly ILog _logger = LogManager.GetLogger("SessionFactory");

        public static string ConnectionString { get; private set; }


        private ISessionFactory CreateSessionFactory(string configFile) {
            _logger.Debug("Creating session factory");
            var configuration = new Configuration();
            configuration.Configure(configFile);

            var db = "";
#if DEBUG
            db = "-test";
#endif

            ConnectionString = configuration.GetProperty("connection.connection_string");
            ConnectionString = ConnectionString.Replace("{appdata}", GlobalPaths.UserdataFolder + @"\");
            ConnectionString = ConnectionString.Replace("{db}", db);
            configuration.SetProperty("connection.connection_string", ConnectionString);


            configuration.AddAssembly(System.Reflection.Assembly.GetEntryAssembly());

            try {
                _logger.Debug("Building session factory");
                var factory = configuration.BuildSessionFactory();

                _logger.Debug("Running schema updates");
                var updater = new SchemaUpdate(configuration);
                updater.Execute(true, true);

                _logger.Info("Database connection established.");
                return factory;
            }
            catch (Exception ex) {
                _logger.Warn(ex.Message);
                _logger.Warn(ex.StackTrace);
                throw;
            }

        }

        private ISessionFactory sessionFactory {
            get {
                if (_sessionFactory == null) {
                    _sessionFactory = CreateSessionFactory("hibernate.sqlite.cfg.xml");
                }
                return _sessionFactory;
            }
        }

        public ISession OpenSession() {
            return sessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession() {
            return sessionFactory.OpenStatelessSession();
        }
    }
}

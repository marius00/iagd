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
using IAGrim.Utilities;

namespace SessionFactoryLoader {
    public class SessionFactory {
        private ISessionFactory _sessionFactory;
        private ILog logger = LogManager.GetLogger("SessionFactory");

        public static string ConnectionString { get; private set; }


        private ISessionFactory CreateSessionFactory(string configFile) {
            // 
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
                var factory = configuration.BuildSessionFactory();

                var updater = new SchemaUpdate(configuration);
                updater.Execute(true, true);

                logger.Info("Database connection established.");
                return factory;
            }
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
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

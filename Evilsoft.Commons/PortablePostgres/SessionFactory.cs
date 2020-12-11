using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using System;
using System.Reflection;
using System.Text;

namespace PortablePostgres {
    public class SessionFactory {
        private static ISessionFactory _sessionFactory;
        private static ILog logger = LogManager.GetLogger("SessionFactory");

        private ISessionFactory CreateSessionFactory(string configFile) {
            if (_sessionFactory == null) {
                var configuration = new Configuration();
                configuration.Configure(configFile);
                configuration.SetProperty("show_sql", "false");

                var db = "";
#if DEBUG
                db = "-test";
#endif

                var connectionString = configuration.GetProperty("connection.connection_string");
                connectionString = connectionString.Replace("{db}", db);
                configuration.SetProperty("connection.connection_string", connectionString);
                //configuration.Configure();
                configuration.AddAssembly(Assembly.GetEntryAssembly());

                try {
                    new SchemaUpdate(configuration).Execute(true, true);
                    _sessionFactory = configuration.BuildSessionFactory();
                }
                catch (Exception ex) {
                    logger.Warn(ex.Message);
                    logger.Warn(ex.StackTrace);
                    throw;
                }
            }
            return _sessionFactory;
        }

        private ISessionFactory sessionFactory {
            get {
                if (_sessionFactory == null) {
                    _sessionFactory = CreateSessionFactory("hibernate.postgres.cfg.xml");
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

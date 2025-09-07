using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using System;
using System.Reflection;
using System.Text;
using EvilsoftCommons;

namespace PortablePostgres {
    public class SessionFactory : ISessionFactoryWrapper {
        private static ISessionFactory _sessionFactory;
        private static readonly ILog Logger = LogManager.GetLogger("SessionFactory");

        private ISessionFactory CreateSessionFactory(string configFile) {
            if (_sessionFactory == null) {
                var configuration = new Configuration();
                configuration.Configure(configFile);

                var db = "";
#if DEBUG
                db = "-test3";
#endif

                var connectionString = configuration.GetProperty("connection.connection_string");
                connectionString = connectionString.Replace("{db}", db);
                configuration.SetProperty("connection.connection_string", connectionString);
                configuration.AddAssembly(Assembly.GetEntryAssembly());

                try {
                    new SchemaUpdate(configuration).Execute(true, true); // Warning: This may silently fail. It just logs and returns, nothing thrown.
                    _sessionFactory = configuration.BuildSessionFactory();
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
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

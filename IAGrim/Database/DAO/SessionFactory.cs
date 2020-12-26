using System;
using NHibernate;
using log4net;
using System.Threading;
using EvilsoftCommons;
using IAGrim.Database.DAO.Util;

namespace IAGrim.Database {
    public interface ISessionCreator {
        ISession OpenSession();
        IStatelessSession OpenStatelessSession();
    }

    public class SessionFactory : ISessionCreator {
        private static ILog Logger = LogManager.GetLogger(typeof(SessionFactory));
        private readonly SqlDialect _dialect;

        public SessionFactory(SqlDialect dialect) {
            _dialect = dialect;
        }

        private ISessionFactoryWrapper CreateSession() {
            if (_dialect == SqlDialect.Sqlite) {
                return new SessionFactoryLoader.SessionFactory();
            }
            else {
                return new PortablePostgres.SessionFactory();
            }
        }

        [ThreadStatic]
        //private static SessionFactoryLoader.SessionFactory sessionFactory;
        private static ISessionFactoryWrapper _sessionFactory;

        static SessionFactory() {
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        public ISession OpenSession() {
            if (_sessionFactory == null) {
                _sessionFactory = CreateSession();
                Logger.Info($"Creating session on thread {Thread.CurrentThread.ManagedThreadId}");
            }

            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "NH:Session";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            //logger.DebugFormat("Session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return _sessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession() {
            if (_sessionFactory == null) {
                _sessionFactory = CreateSession();
                Logger.Info($"Creating session on thread {Thread.CurrentThread.ManagedThreadId}");
            }

            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "NH:Session";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            //logger.DebugFormat("Stateless session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return _sessionFactory.OpenStatelessSession();
        }
    }
}
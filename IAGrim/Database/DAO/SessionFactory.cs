using System;
using NHibernate;
using log4net;
using System.Threading;
using EvilsoftCommons;
using IAGrim.Database.DAO.Util;

namespace IAGrim.Database {


    public class SessionFactory {
        private static ILog Logger = LogManager.GetLogger(typeof(SessionFactory));

        // NHibernate's ISessionFactory is thread-safe and is meant to be built once and
        // shared across the whole application (unlike ISession, which must stay confined
        // to a single thread/unit-of-work and is opened per-call via OpenSession()).
        private static readonly Lazy<ISessionFactoryWrapper> _sessionFactory =
            new Lazy<ISessionFactoryWrapper>(CreateSession, LazyThreadSafetyMode.ExecutionAndPublication);

        private static ISessionFactoryWrapper CreateSession() {
            Logger.Info($"Creating session factory on thread {Thread.CurrentThread.ManagedThreadId}");
            return new SessionFactoryLoader.SessionFactory();
        }

        static SessionFactory() {
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        private static void NameCurrentThreadIfUnnamed() {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "NH:Session";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }

        public ISession OpenSession() {
            NameCurrentThreadIfUnnamed();

            //logger.DebugFormat("Session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return _sessionFactory.Value.OpenSession();
        }

        public IStatelessSession OpenStatelessSession() {
            NameCurrentThreadIfUnnamed();

            //logger.DebugFormat("Stateless session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return _sessionFactory.Value.OpenStatelessSession();
        }
    }
}

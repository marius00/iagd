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

        /// <summary>
        /// Builds the shared factory (parsing the config and compiling the hbm mappings) without opening a
        /// connection, so it can be done ahead of time on another thread.
        ///
        /// Deliberately does not open a session: the first thing that runs against the database is the journal-mode
        /// migration, and "PRAGMA journal_mode = WAL" can come back busy while another connection is alive.
        /// </summary>
        public static void Warmup() {
            NameCurrentThreadIfUnnamed();
            _ = _sessionFactory.Value;
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

        /// <summary>
        /// Truncates the SQLite write-ahead log back into the main database file.
        /// WAL mode is enabled (see EnableWalJournalMode) but never checkpointed otherwise, so the
        /// -wal file grows unbounded across sessions - especially when the process is killed rather
        /// than closed cleanly. A large -wal makes the first queries on the next launch extremely
        /// slow (observed as an ~11s stall during startup on a fully populated collection).
        /// </summary>
        public void Checkpoint() {
            try {
                using ISession session = OpenSession();
                session.CreateSQLQuery("PRAGMA wal_checkpoint(TRUNCATE);").List();
            }
            catch (Exception ex) {
                Logger.Warn("Failed to checkpoint the WAL", ex);
            }
        }
    }
}

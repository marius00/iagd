using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using IAGrim.Utilities;
using EvilsoftCommons.Exceptions;
using System.Security.Permissions;
using System.Security;
using System.Threading;

namespace IAGrim.Database {
    public interface ISessionCreator {

        ISession OpenSession();

        IStatelessSession OpenStatelessSession();
    }

    public class SessionFactory : ISessionCreator {
        private static ILog logger = LogManager.GetLogger(typeof(SessionFactory));

        [ThreadStatic]
        private static SessionFactoryLoader.SessionFactory sessionFactory;

        static SessionFactory() {
            System.Net.ServicePointManager.Expect100Continue = false;
        }
        
        private class Migrator<T> where T : PlayerItem {
            public static bool Execute (ISessionCreator factory, ISessionFactory legacySessionFactory, bool isHardcore) {
                try {
                    IList<T> elements;
                    using (var legacy = legacySessionFactory.OpenSession()) {
                        elements = legacy.CreateCriteria<T>().List<T>();
                    }


                    using (var session = factory.OpenSession()) {
                        using (var transaction = session.BeginTransaction()) {
                            session.CreateQuery("DELETE FROM PlayerItemStat").ExecuteUpdate();
                            session.CreateQuery("DELETE FROM PlayerItem").ExecuteUpdate();

                            foreach (var item in elements) {
                                PlayerItem copy = new PlayerItem {
                                    BaseRecord = item.BaseRecord,
                                    EnchantmentRecord = item.EnchantmentRecord,
                                    EnchantmentSeed = item.EnchantmentSeed,
                                    MateriaCombines = item.MateriaCombines,
                                    MateriaRecord = item.MateriaRecord,
                                    ModifierRecord = item.ModifierRecord,
                                    PrefixRecord = item.PrefixRecord,
                                    RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                                    RelicSeed = item.RelicSeed,
                                    Seed = item.Seed,
                                    StackCount = Math.Max(1, item.StackCount),
                                    SuffixRecord = item.SuffixRecord,
                                    TransmuteRecord = item.TransmuteRecord,
                                    UNKNOWN = item.UNKNOWN,
                                    Mod = item.Mod,
                                    IsHardcore = isHardcore,
                                    //IsExpansion1 = item.IsExpansion1
                                };

                                session.Save(copy);
                            }
                            transaction.Commit();
                        }
                    }

                    return true;
                } catch (Exception ex) {
                    logger.Warn(ex.Message);
                    logger.Warn(ex.StackTrace);
                    return false;
                }
            }
        }
        public static bool Migrate(ISessionCreator factory) {
            string[] files = {
                Path.Combine(GlobalPaths.UserdataFolder, GlobalPaths.USERDATA_FILE),
                Path.Combine(GlobalPaths.UserdataFolder, "userdata-hc.db")
            };

            foreach (var dbfile in files) {
                if (!File.Exists(dbfile)) {
                    logger.Info("Migration requested but already performed. Halted.");
                    return true;
                }

                if (!Migrator<PlayerItem>.Execute(factory, CreateLegacySessionFactory(dbfile), dbfile.Contains("-hc"))) {
                    logger.Warn("Failed to items");
                    return false;
                } else {
                    // Rename it, so its not migrated again.
                    if (File.Exists(string.Format("{0}.legacy", dbfile)))
                        File.Delete(string.Format("{0}.legacy", dbfile));
                    File.Move(dbfile, string.Format("{0}.legacy", dbfile));
                    logger.Info("Migration successful, old database renamed to .legacy");
                }
            }

            return true;
        }

        private static ISessionFactory CreateLegacySessionFactory(string file) {
            try {
                var legacyConnectionString = string.Format("Data Source = {0};Version=3", file);


                var configuration = new Configuration();
                configuration.Configure();
                configuration.SetProperty("connection.connection_string", legacyConnectionString);
                configuration.AddAssembly(Assembly.GetExecutingAssembly());

                try {
                    new SchemaUpdate(configuration).Execute(true, true);
                    return configuration.BuildSessionFactory();
                }
                catch (Exception ex) {
                    logger.Warn(ex.Message);
                    logger.Warn(ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                    throw;
                }                
            }
            catch (System.Reflection.TargetInvocationException ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex.InnerException);
                ExceptionReporter.ReportException(ex, "[Outer Exception]", true);
                throw;
            }
        }
        

        public ISession OpenSession() {
            if (sessionFactory == null) {
                sessionFactory = new SessionFactoryLoader.SessionFactory();
                logger.Info($"Creating session on thread {Thread.CurrentThread.ManagedThreadId}");
            }

            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "NH:Session";

            //logger.DebugFormat("Session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return sessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession() {
            if (sessionFactory == null) {
                sessionFactory = new SessionFactoryLoader.SessionFactory();
                logger.Info($"Creating session on thread {Thread.CurrentThread.ManagedThreadId}");
            }

            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "NH:Session";

            //logger.DebugFormat("Stateless session opened on thread {0}, Stacktrace: {1}", System.Threading.Thread.CurrentThread.Name, new System.Diagnostics.StackTrace());
            return sessionFactory.OpenStatelessSession();
        }
    }
}

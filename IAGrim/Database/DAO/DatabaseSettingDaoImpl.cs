using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling internal Grim Dawn items
    /// These are not user owned items
    /// </summary>
    public class DatabaseSettingDaoImpl : BaseDao<DatabaseSetting>, IDatabaseSettingDao {
        private static ILog logger = LogManager.GetLogger(typeof(DatabaseSettingDaoImpl));

        public DatabaseSettingDaoImpl(ISessionCreator sessionCreator) : base (sessionCreator) {
        }



        /// <summary>
        /// Get the timestamp for the last database update (GD database)
        /// </summary>
        /// <returns></returns>
        public long GetLastDatabaseUpdate() {
            using (ISession session = SessionCreator.OpenSession()) {
                object result = session.CreateCriteria<DatabaseSetting>().Add(Restrictions.Eq("Id", "LastUpdate")).UniqueResult();
                if (result != null) {
                    DatabaseSetting s = result as DatabaseSetting;
                    return s.V1;
                }
            }

            return 0L;
        }

        /// <summary>
        /// Get the timestamp for the last recipe update
        /// </summary>
        /// <returns></returns>
        /*
        public long GetLastRecipeUpdate() {
            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    object result = session.CreateCriteria<DatabaseSetting>().Add(Restrictions.Eq("Id", "LastRecipeUpdate")).UniqueResult();
                    if (result != null) {
                        DatabaseSetting s = result as DatabaseSetting;
                        return s.V1;
                    }
                }
            }

            return 0L;
        }*/

        /// <summary>
        /// Store the current database path
        /// Mainly just for displaying the currently loaded mod (if any)
        /// </summary>
        /// <param name="path"></param>
        public void UpdateCurrentDatabase(string path) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    bool updated = session
                        .CreateQuery("UPDATE DatabaseSetting SET V2 = :value WHERE Id = 'CurrentDatabase'")
                        .SetParameter("value", path).ExecuteUpdate() != 0;

                    if (!updated) {
                        session.Save(new DatabaseSetting { Id = "CurrentDatabase", V2 = path });
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Get the path to the latest database parsed
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDatabasePath() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    object result = session.CreateCriteria<DatabaseSetting>()
                        .Add(Restrictions.Eq("Id", "CurrentDatabase"))
                        .UniqueResult();


                    if (result != null) {
                        return (result as DatabaseSetting).V2;
                    }
                }
            }

            return string.Empty;
        }


        public void UpdateRecipeTimestamp(long ts) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (session.CreateQuery("UPDATE DatabaseSetting SET V1 = :value WHERE Id = 'LastRecipeUpdate'")
                        .SetParameter("value", ts).ExecuteUpdate() == 0) {

                            session.Save(new DatabaseSetting { Id = "LastRecipeUpdate", V1 = ts });
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Update the internal timestamp indicating that a database update has taken place
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public void UpdateDatabaseTimestamp(long ts) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (session.CreateQuery("UPDATE DatabaseSetting SET V1 = :value WHERE Id = 'LastUpdate'")
                        .SetParameter("value", ts)
                        .ExecuteUpdate() == 0) {

                            session.Save(new DatabaseSetting { Id = "LastUpdate", V1 = ts });
                    }

                    transaction.Commit();
                }
            }
        }

        public string GetUuid() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    object result = session.CreateCriteria<DatabaseSetting>()
                        .Add(Restrictions.Eq("Id", "Uuid"))
                        .UniqueResult();


                    if (result != null) {
                        return (result as DatabaseSetting).V2;
                    }
                }
            }

            return string.Empty;
        }

        public void SetUuid(string uuid) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (session.CreateQuery("UPDATE DatabaseSetting SET V2 = :value WHERE Id = 'Uuid'")
                            .SetParameter("value", uuid)
                            .ExecuteUpdate() == 0) {

                        session.Save(new DatabaseSetting { Id = "Uuid", V2 = uuid });
                    }

                    transaction.Commit();
                }
            }

        }
    }
}

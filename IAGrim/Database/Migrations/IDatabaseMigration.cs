using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Migrations {
    abstract class IDatabaseMigration {
        abstract public void Migrate(SessionFactory sessionCreator);

        public static bool ColumnExists(SessionFactory sessionCreator, string table, string column) {
            using (ISession session = sessionCreator.OpenSession()) {
                foreach (var row in session.CreateSQLQuery($"PRAGMA table_info('{table}')").List()) {
                    var arr = (object[])row;
                    int cid = (int)(System.Int64)arr[0];
                    string name = (string)arr[1];
                    string type = (string)arr[2];
                    bool notNull = (int)(System.Int64)arr[3] == 1;

                    if (name == column) {
                        return true;
                    }

                }
            }

            return false;
        }

        public static bool TableExists(SessionFactory sessionCreator, string table) {
            using (ISession session = sessionCreator.OpenSession()) {
                foreach (var row in session.CreateSQLQuery($"SELECT * FROM sqlite_master WHERE type='table' and name = :name").SetParameter("name", table).List()) {
                    return true;
                }
            }

          
            return false;
        }
        public static bool IndexExists(SessionFactory sessionCreator, string index) {
            using (ISession session = sessionCreator.OpenSession()) {
                foreach (var row in session.CreateSQLQuery($"SELECT * FROM sqlite_master WHERE type='index' and name = :name").SetParameter("name", index).List()) {
                    return true;
                }
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.DAO.Util {
    static class SqlUtils {
        public static string EnsureDialect(SqlDialect dialect, string query) {
            if (dialect == SqlDialect.Sqlite)
                return query;
            else {
                if (query.IndexOf("INSERT OR IGNORE") != -1) {
                    return query.Replace("INSERT OR IGNORE", "INSERT") + " ON CONFLICT DO NOTHING";
                }

                return query;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Interfaces;

namespace IAGrim.Database.DAO {
    class AzurePartitionDaoImpl : BaseDao<AzurePartition>, IAzurePartitionDao {
        public AzurePartitionDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {
        }
    }
}

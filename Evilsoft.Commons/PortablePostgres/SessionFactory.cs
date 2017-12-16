using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PortablePostgres {
    public class SessionFactory {
        private static ISessionFactory _sessionFactory;
        private static ILog logger = LogManager.GetLogger("SessionFactory");

        private static ISessionFactory sessionFactory {
            get {
                if (_sessionFactory == null) {
                    var configuration = new Configuration();

                    configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                    configuration.SetProperty("dialect", "NHibernate.Dialect.PostgreSQL81Dialect");
                    configuration.SetProperty("connection.driver_class", "NHibernate.Driver.NpgsqlDriver");
                    configuration.SetProperty("show_sql", "false");

                    configuration.SetProperty("connection.connection_string", string.Format("server=localhost;Port=5559;Database=app;User Id={0};Password=", System.Environment.UserName));
                    //configuration.Configure();
                    configuration.AddAssembly(Assembly.GetExecutingAssembly());
                    //configuration.SetProperty("connection.connection_string", "server=localhost;Port=5432;Database=app;User Id=pagloja;Password=123456;");

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
        }

        public static ISession OpenSession() {
            return sessionFactory.OpenSession();
        }
        public static IStatelessSession OpenStatelessSession() {
            return sessionFactory.OpenStatelessSession();
        }
    }
}

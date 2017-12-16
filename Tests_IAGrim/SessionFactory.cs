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
using System.Security.Permissions;
using System.Security;

namespace IAGrim.Tests {

    public class SessionFactory : Database.ISessionCreator {
        private static ILog logger = LogManager.GetLogger(typeof(SessionFactory));
        private static ISessionFactory _sessionFactory;
        private static string _dbfile;
        ~SessionFactory() {
            if (!string.IsNullOrEmpty(_dbfile) && File.Exists(_dbfile)) {
                try {
                    File.Delete(_dbfile);
                } catch (Exception) { /* Don't care */ }
            }
        }


        private static string ConnectionString {
            get {
                if (string.IsNullOrEmpty(_dbfile))
                    _dbfile = Path.GetTempFileName();

                return string.Format("Data Source = {0};Version=3", _dbfile);
            }
        }



        private static ISessionFactory sessionFactory {
            get {
                try {
                    if (_sessionFactory == null) {
                        var configuration = new Configuration();
                        configuration.Configure();
                        configuration.AddAssembly(Assembly.GetExecutingAssembly());
                        configuration.SetProperty("connection.connection_string", ConnectionString);
                        configuration.AddAssembly("IAGrim");
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
                catch (System.Reflection.TargetInvocationException ex) {
                    logger.Warn(ex.Message);
                    logger.Warn(ex.StackTrace);
                    throw;
                }
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

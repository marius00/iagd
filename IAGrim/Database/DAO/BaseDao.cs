using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using IAGrim.Database.Interfaces;

namespace IAGrim.Database {


    public class BaseDao<T> where T : class {

        protected readonly ISessionCreator SessionCreator;

        protected BaseDao(ISessionCreator sessionCreator) {
            this.SessionCreator = sessionCreator;
        }


        public long GetNumItems() {
            using (var session = SessionCreator.OpenSession()) {
                return session.QueryOver<T>()
                    .ToRowCountInt64Query()
                    .SingleOrDefault<long>();
            }
        }

        public virtual void Save(T obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.Save(obj);
                    transaction.Commit();
                }
            }
        }
        public virtual void Save(IEnumerable<T> objs) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var obj in objs) {
                        session.Save(obj);
                    }
                    transaction.Commit();
                }
            }
        }


        protected struct Parameter {
            public Parameter(string field, object value) {
                this.field = field;
                this.value = value;
            }

            public string field;
            public object value;
        }

        protected void SingleQuery(String hql, Parameter[] bindings) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var query = session.CreateQuery(hql);
                    foreach (var binding in bindings) {
                        query.SetParameter(binding.field, binding.value);
                    }
                    query.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }
        protected void SingleQuery(String hql, Parameter binding) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var query = session.CreateQuery(hql);
                    query.SetParameter(binding.field, binding.value);
                    query.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        public virtual IList<T> ListAll() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<T>().List<T>();
                }
            }
        }


        protected void SingleQuery(String hql, List<Parameter> bindings) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var query = session.CreateQuery(hql);
                    foreach (var binding in bindings) {
                        query.SetParameter(binding.field, binding.value);
                    }
                    query.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        public virtual void Update(T obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.Update(obj);
                    transaction.Commit();
                }
            }
        }


        public virtual void SaveOrUpdate(T obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.SaveOrUpdate(obj);
                    transaction.Commit();
                }
            }
        }
        public virtual void SaveOrUpdate(IEnumerable<T> objs) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var obj in objs) {
                        session.SaveOrUpdate(obj);
                    }
                    transaction.Commit();
                }
            }
        }

        public virtual void Remove(IEnumerable<T> objs) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (T obj in objs) {
                        session.Delete(obj);
                    }
                    transaction.Commit();
                }
            }
        }

        public virtual void Remove(T obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.Delete(obj);
                    transaction.Commit();
                }
            }
        }

        public virtual T GetById(long id) {
            using (ISession session = SessionCreator.OpenSession()) {
                return session.Get<T>(id);
            }
        }

    }
    
}

using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Synchronizer {
    public class BasicSynchronizer<T> : IBaseDao<T> {
        protected readonly ThreadExecuter ThreadExecuter;
        protected readonly ISessionCreator SessionCreator;
        protected IBaseDao<T> BaseRepo;

        public BasicSynchronizer(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) {
            this.ThreadExecuter = threadExecuter;
            this.SessionCreator = sessionCreator;
        }

        public long GetNumItems() {
            return ThreadExecuter.Execute(
                () => BaseRepo.GetNumItems()
            );
        }

        public void Save(T obj) {
            ThreadExecuter.Execute(
                () => BaseRepo.Save(obj)
            );
        }

        public void Save(IEnumerable<T> objs) {
            ThreadExecuter.Execute(
                () => BaseRepo.Save(objs)
            );
        }

        public void Update(T obj) {
            ThreadExecuter.Execute(
                () => BaseRepo.Update(obj)
            );
        }

        public void SaveOrUpdate(T obj) {
            ThreadExecuter.Execute(
                () => BaseRepo.SaveOrUpdate(obj)
            );
        }

        public void SaveOrUpdate(IEnumerable<T> objs) {
            ThreadExecuter.Execute(
                () => BaseRepo.SaveOrUpdate(objs)
            );
        }

        public void Remove(IEnumerable<T> objs) {
            ThreadExecuter.Execute(
                () => BaseRepo.Remove(objs)
            );
        }

        public void Remove(T obj) {
            ThreadExecuter.Execute(
                () => BaseRepo.Remove(obj)
            );
        }

        public T GetById(long id) {
            return ThreadExecuter.Execute(
                () => BaseRepo.GetById(id)
            );
        }

        public IList<T> ListAll() {
            return ThreadExecuter.Execute(
                () => BaseRepo.ListAll()
            );
        }
    }
}

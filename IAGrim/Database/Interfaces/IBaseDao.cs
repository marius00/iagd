using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Interfaces {
    public interface IBaseDao<T> {

        long GetNumItems();

        void Save(T obj);
        void Save(IEnumerable<T> objs);

        void Update(T obj);

        IList<T> ListAll();

        void SaveOrUpdate(T obj);
        void SaveOrUpdate(IEnumerable<T> objs);

        void Remove(IEnumerable<T> objs);

        void Remove(T obj);

        T GetById(long id);
    }
}

using NHibernate;

namespace EvilsoftCommons {
    public interface ISessionFactoryWrapper {
        ISession OpenSession();
        IStatelessSession OpenStatelessSession();
    }
}

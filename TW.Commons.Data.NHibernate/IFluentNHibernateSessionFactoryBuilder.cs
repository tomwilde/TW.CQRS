using System.Reflection;
using NHibernate;

namespace TW.Commons.Data.NHibernate
{
    public interface IFluentNHibernateSessionFactoryBuilder
    {
        FluentNHibernateSessionFactoryBuilder ConfigureUsingType<T>();
        ISessionFactory Build();
        void ConfigureMappingsUsingAssembly(Assembly assembly);
    }
}
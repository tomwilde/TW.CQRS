using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using TW.Commons.Interfaces;

namespace TW.Commons.Data.NHibernate
{
    public class FluentNHibernateSessionFactoryBuilder : IFluentNHibernateSessionFactoryBuilder
    {
        private readonly string connectionString;

        private readonly object lockr = new object();
        private ISessionFactory factory;

        private readonly List<Type> mappingAssemblyTypeRefs = new List<Type>();

        public FluentNHibernateSessionFactoryBuilder(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionString = connectionStringProvider.Get("events");
        }

        public FluentNHibernateSessionFactoryBuilder ConfigureUsingType<T>()
        {
            mappingAssemblyTypeRefs.Add(typeof(T));
            return this;
        }

        public ISessionFactory Build()
        {
            // we only build it once...
            lock(lockr)
            {
                if (factory == null)
                {
                    var i = Fluently.Configure()
                                   .Diagnostics(x => x.Enable())
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString)));

                    mappingAssemblyTypeRefs.ForEach(t => i.Mappings(mm => mm.FluentMappings.AddFromAssembly(Assembly.GetAssembly(t))));

                    factory =     i.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                                   .BuildSessionFactory();
                }
            }

            return factory;
        }

        public void ConfigureMappingsUsingAssembly(Assembly assembly)
        {
            mappingAssemblyTypeRefs.Add(assembly.GetTypes().First());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace TW.CQRS.Core.Reporting
{
    public class FluentNHibernateSessionFactoryBuilder
    {
        private readonly string connectionString;

        private object lockr = new object();
        private ISessionFactory factory;

        public FluentNHibernateSessionFactoryBuilder(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ISessionFactory ConfigureUsingType<T>()
        {
            // we only build it once...
            lock(lockr)
            {
                if (factory == null)
                {
                    factory = Fluently.Configure()
                                   .Diagnostics(x => x.Disable())
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString)))
                                   .Mappings(m => m.FluentMappings.AddFromAssemblyOf<T>())
                                   .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                                   .BuildSessionFactory();
                }
            }

            return factory;
        }
    }
}

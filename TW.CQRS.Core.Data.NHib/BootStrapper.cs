using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using TW.Commons.Data.NHibernate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.NHib
{
    public static class BootStrapper
    {
        public static bool Configured(IUnityContainer container)
        {
            return container.Registrations.Any(x => x.RegisteredType == typeof(IFluentNHibernateSessionFactoryBuilder));
        }

        public static void RegisterForEventStorage(IUnityContainer container, List<Assembly> mappingAssemblies = null)
        {
            ConfigureIfRequired(container, mappingAssemblies);
            container.RegisterType<IAggregateRootEventStorage, NHibAggregateRootEventStorage>();
        }

        public static void RegisterForReporting(IUnityContainer container, List<Assembly> mappingAssemblies = null)
        {
            ConfigureIfRequired(container, mappingAssemblies);
            container.RegisterType<IReportingRepository, NHibReportingRepository>();
        }

        private static void ConfigureIfRequired(IUnityContainer container, List<Assembly> mappingAssemblies)
        {
            if (!Configured(container))
            {
                container.RegisterType<IFluentNHibernateSessionFactoryBuilder, FluentNHibernateSessionFactoryBuilder>();

                var sessionFactoryBuilder = container.Resolve<IFluentNHibernateSessionFactoryBuilder>();

                // custom mappings
                mappingAssemblies = mappingAssemblies ?? new List<Assembly>();
                mappingAssemblies.ForEach(sessionFactoryBuilder.ConfigureMappingsUsingAssembly);

                // standard mappings
                sessionFactoryBuilder.ConfigureUsingType<Mappings.EventMap>();

                // build 'er up
                container.RegisterInstance(sessionFactoryBuilder.Build());

                // supporing svcs
                container.RegisterType<IGenericEntityService, GenericEntityService>();
            }
        }
    }
}
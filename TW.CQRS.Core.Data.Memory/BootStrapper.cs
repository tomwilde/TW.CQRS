using Microsoft.Practices.Unity;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Reporting;

using Singleton = Microsoft.Practices.Unity.ContainerControlledLifetimeManager;

namespace TW.CQRS.Core.Data.Memory
{
    public static class BootStrapper
    {
        public static void RegisterForEventStorage(IUnityContainer container)
        {
            container.RegisterType<IAggregateRootEventStorage, MemoryAggregateRootEventStorage>();
        }
        
        public static void RegisterForReporting(IUnityContainer container)
        {
            container.RegisterType<IReportingRepository, MemoryReportingRepository>(new Singleton());
        }
    }
}
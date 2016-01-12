using Microsoft.Practices.Unity;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.Sql
{
    public static class BootStrapper
    {
        public static void RegisterForEventStorage(IUnityContainer container)
        {
            container.RegisterType<IAggregateRootEventStorage, SqlAggregateRootEventStorage>();
        }

        public static void RegisterForReporting(IUnityContainer container)
        {
            container.RegisterType<IReportingRepository, SqlReportingRepository>();
        }
    }
}
using Microsoft.Practices.Unity;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Domain.Command;
using TW.CQRS.Core.Domain.Event;
using TW.CQRS.Core.Domain.Reporting;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.CQRS.Core.Interfaces.Reporting;
using Singleton = Microsoft.Practices.Unity.ContainerControlledLifetimeManager;

namespace TW.CQRS.Core
{
    public static class BootStrapper
    {
        /// <summary>
        /// We encapsulate IOC registration... :)
        /// </summary>
        public static void Register(IUnityContainer container)
        {
            // stateless so dont need a singleton...
            container.RegisterType<IDomainCommandBus, DomainCommandBus>();
            container.RegisterType<IDomainEventBus, DomainEventBus>();
            container.RegisterType<IReportingUpdatesBus, ReportingUpdatesBus>(new Singleton());

            container.RegisterType<DomainCommandHandlerFactory>(new Singleton());
            container.RegisterType<DomainEventHandlerFactory>(new Singleton());

            container.RegisterType<IEventToAggregateEventMapper, EventToAggregateEventMapper>();
        }
    }
}
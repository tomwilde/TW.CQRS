using EstateAgent.Domain.CommandHandlers;
using EstateAgent.Domain.EventHandlers;
using EstateAgent.Domain.Normalisers;
using Microsoft.Practices.Unity;
using TW.CQRS.Core.Domain.Command;
using TW.CQRS.Core.Domain.Event;
using TW.CQRS.Core.Interfaces;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.CQRS.Core.Interfaces.Domain.Event;

namespace EstateAgent.Domain
{

    public class BootStrapper
    {
        public static void Register(IUnityContainer container)
        {
            // Domain Commands - We dont need anything else here.. we simply raise commands into the domain.
            container.RegisterType<IDomainCommandHandler<Commands.AddNewPropertyDomainCommand>, AddNewPropertyDomainCommandHandler>();

            // Domain Events
            container.RegisterType<IDomainEventHandler<Events.PropertyRegisteredDomainEvent>, PropertyRegisteredDomainEventHandler>();

            // Normaliser
            container.RegisterType<INormaliser, EstateAgentNormaliser>();

            // Startup code...
            // TODO: Register a factory for the Command and Events to remove the need for an initialiser....
            container.Resolve<DomainCommandHandlerFactory>().Initialise(container);
            container.Resolve<DomainEventHandlerFactory>().Initialise(container);
        }
    }
}
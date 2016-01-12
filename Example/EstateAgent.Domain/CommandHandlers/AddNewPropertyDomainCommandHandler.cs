using EstateAgent.Domain.Commands;
using EstateAgent.Domain.Events;

using TW.CQRS.Core.Domain.Command;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.Commons.Interfaces;
using EstateAgent.Domain.Model;


namespace EstateAgent.Domain.CommandHandlers
{
    public class AddNewPropertyDomainCommandHandler : AbstractDomainCommandHandler<AddNewPropertyDomainCommand>
    {
        private readonly IAggregateRootEventStorage rootEventStore;
        private readonly IDomainEventBus domainEventBus;

        public AddNewPropertyDomainCommandHandler(ILoggerFactory loggerFactory, 
                                                  IAggregateRootEventStorage rootEventStore,
                                                  IDomainEventBus domainEventBus)
            : base(loggerFactory)
        {
            this.rootEventStore = rootEventStore;
            this.domainEventBus = domainEventBus;
        }

        public override void Handle(AddNewPropertyDomainCommand command)
        {
            WithinTransaction(command, cmd =>
            {
                // *Actual Processing* 
                var property = Property.Create(cmd.Name, cmd.Price);
                // /*Actual Processing*  (did you miss it?)

                // Capture the aggregate events
                rootEventStore.SaveEvents(property);
                
                // Raise the fact we created a property externally
                domainEventBus.Publish(new PropertyRegisteredDomainEvent(cmd.Id, cmd.Name, cmd.Price));
            });
        }
    }
}
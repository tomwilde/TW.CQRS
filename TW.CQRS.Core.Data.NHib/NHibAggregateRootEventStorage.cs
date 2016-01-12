using System.Collections.Generic;
using System.Linq;
using TW.Commons.Data.NHibernate;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Data.NHib
{
    public class NHibAggregateRootEventStorage : IAggregateRootEventStorage
    {
        private readonly IEventToAggregateEventMapper _eventToAggregateEventMapper;
        private readonly ISerialiser _serialiser;
        private readonly IGenericEntityService _genericEntityService;

        private ILogger _logger;

        public NHibAggregateRootEventStorage(ILoggerFactory loggerFactory
                                           , IEventToAggregateEventMapper eventToAggregateEventMapper
                                           , ISerialiser serialiser
                                           , IGenericEntityService genericEntityService)
        {
            _eventToAggregateEventMapper = eventToAggregateEventMapper;
            _serialiser = serialiser;
            _genericEntityService = genericEntityService;
            _logger = loggerFactory.Create();
        }

        public LinkedList<IAggregateEvent> GetEvents(string aggregateName)
        {
            var aggregateId = int.Parse(aggregateName);
            var list = _genericEntityService.Get<Event>(q => q.Where(x => x.AggregateId == aggregateId).ToList());

            return new LinkedList<IAggregateEvent>(_eventToAggregateEventMapper.Map(list));
        }


        public void SaveEvents(IAggregateRoot aggregateRoot)
        {
            var events = new List<Event>();

            var changes = aggregateRoot.GetAllChanges();

            var enumerator = changes.GetEnumerator();
            var count = 0;

            while (enumerator.MoveNext())
            {
                count++;
                var @event = enumerator.Current;

                events.Add(new Event()
                {
                    AggregateId = @event.AggregateId,
                    EventType = @event.GetType().AssemblyQualifiedName,
                    EventData = _serialiser.SerialiseAs(@event.GetType(), @event),
                    Timestamp = @event.TimeStamp
                });

                _genericEntityService.SaveOrUpdateEntities(events);
            }
        }
    }
}

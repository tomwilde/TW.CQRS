using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Data.Memory
{
    public class MemoryAggregateRootEventStorage : IAggregateRootEventStorage
    {
        private readonly ILogger logger;
        private readonly ISerialiser _serialiser;
        private readonly IEventToAggregateEventMapper _eventToAggregateEventMapper;

        private readonly List<Event> allEvents = new List<Event>();

        public MemoryAggregateRootEventStorage(ILoggerFactory loggerFactory
                                             , ISerialiser serialiser
                                             , IEventToAggregateEventMapper eventToAggregateEventMapper)
        {
            _serialiser = serialiser;
            _eventToAggregateEventMapper = eventToAggregateEventMapper;

            logger = loggerFactory.Create();
        }

        public LinkedList<IAggregateEvent> GetEvents(string aggregateName)
        {
            var aggregateId = int.Parse(aggregateName);

            var events = allEvents.Select(x => x.Clone())
                                  .Cast<Event>()
                                  .Where(x => x.AggregateId == aggregateId)
                                  .ToList();
            
            return _eventToAggregateEventMapper.Map(events);
        }

        public void SaveEvents(IAggregateRoot aggregateRoot)
        {
            var changes = aggregateRoot.GetAllChanges();

            var enumerator = changes.GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    var @event = enumerator.Current;

                    allEvents.Add(new Event()
                    {
                        AggregateId = @event.AggregateId,
                        EventType = @event.GetType().AssemblyQualifiedName,
                        EventData = _serialiser.SerialiseAs(@event.GetType(), @event),
                        Timestamp = @event.TimeStamp
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.ErrorFormat("Failed to save events!");
                throw;
            }
        }
    }
}

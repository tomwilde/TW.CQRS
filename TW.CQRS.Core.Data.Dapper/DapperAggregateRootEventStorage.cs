using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Data.Dapper
{
    public class DapperAggregateRootEventStorage : IAggregateRootEventStorage
    {
        private readonly ILogger logger;
        private readonly ISerialiser _serialiser;
        private readonly IEventToAggregateEventMapper _eventToAggregateEventMapper;
        private readonly string _connectionString;

        private const string SELECT_ALL_EVENTS = "SELECT EventType, EventData, Timestamp FROM tblEvent WHERE AggregateId = @AggregateId ORDER BY Timestamp";
        private const string SAVE_EVENT = "INSERT INTO tblEvent (AggregateId, EventType, EventData, Timestamp) VALUES (@AggregateId, @EventType, @EventData, @TimeStamp)";

        public DapperAggregateRootEventStorage(ILoggerFactory loggerFactory
                                             , ISerialiser serialiser
                                             , IConnectionStringProvider connectionStringProvider
                                             , IEventToAggregateEventMapper eventToAggregateEventMapper)
        {
            _serialiser = serialiser;
            _eventToAggregateEventMapper = eventToAggregateEventMapper;

            logger = loggerFactory.Create();
            _connectionString = connectionStringProvider.Get("events");
        }

        public LinkedList<IAggregateEvent> GetEvents(string aggregateName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var aggregateId = int.Parse(aggregateName);

                return _eventToAggregateEventMapper.Map(connection.Query<Event>(SELECT_ALL_EVENTS, new { AggregateId = aggregateId }).ToList());
            }
        }

        public void SaveEvents(IAggregateRoot aggregateRoot)
        {
            var changes = aggregateRoot.GetAllChanges();

            var enumerator = changes.GetEnumerator();
            var count = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); 
                var transaction = connection.BeginTransaction();

                while (enumerator.MoveNext())
                {
                    count++;
                    var @event = enumerator.Current;

                    var eventToSave = new Event()
                    {
                        AggregateId = @event.AggregateId,
                        EventType = @event.GetType().AssemblyQualifiedName,
                        EventData = _serialiser.SerialiseAs(@event.GetType(), @event),
                        Timestamp = @event.TimeStamp
                    };

                    connection.Execute(SAVE_EVENT, new
                    {
                        AggregateId = eventToSave.AggregateId,
                        EventType = eventToSave.EventType,
                        EventData = @eventToSave.EventData,
                        TimeStamp = eventToSave.Timestamp,
                    });
                }

                transaction.Commit();
                connection.Close();
            }
        }
    }
}

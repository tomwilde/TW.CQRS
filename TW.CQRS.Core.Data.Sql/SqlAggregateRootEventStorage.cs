using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;
using TW.Commons.Interfaces;
using TW.Commons.Serialisation;

namespace TW.CQRS.Core.Data.Sql
{
    public class SqlAggregateRootEventStorage : IAggregateRootEventStorage
    {
        private readonly ILogger logger;

        private readonly Serialiser serialiser = new Serialiser();

        private bool tableAvailable;
        private readonly string _connectionString;

        private const string TRUNCATE_TBL = "TRUNCATE TABLE tblEvent";
        private const string SAVE_EVENT = "INSERT INTO tblEvent (AggregateId, EventType, EventData, Timestamp) VALUES (@AggregateId, @EventType, @EventData, @TimeStamp)";
        private const string SELECT_ALL_EVENTS = "SELECT EventType, EventData, Timestamp FROM tblEvent WHERE AggregateId = '{0}' ORDER BY Timestamp";
        private const string LOCATE_TABLE = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblEvent'";
        
        // shouldnt need this as NHib will automagically create it
        private const string CREATE_TABLE = @"
CREATE TABLE [dbo].[tblEvent] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AggregateId] INT            NULL,
    [EventData]   NVARCHAR (MAX) NULL,
    [EventType]   NVARCHAR (MAX) NULL,
    [Timestamp]   DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
";

        public SqlAggregateRootEventStorage(ILoggerFactory loggerFactory, IConnectionStringProvider connectionStringProvider)
        {
            logger = loggerFactory.Create();
            _connectionString = connectionStringProvider.Get("events");
        }

        private void CreateTableIfRequired()
        {
            if (tableAvailable)
            {
                logger.InfoFormat("Events table present...");
                return;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = LOCATE_TABLE;

                using (var command = new SqlCommand(query))
                {
                    command.Connection = connection;
                    var reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        reader.Close();

                        var createTable = string.Format(CREATE_TABLE);
                        using (var cmd = new SqlCommand(createTable))
                        {
                            cmd.Connection = connection;
                            cmd.ExecuteNonQuery();
                            logger.Info("Created Events table");
                            tableAvailable = true;
                        }
                    }
                }
            }            
        }

        private void TruncateTable()
        {
            logger.Info("Truncating events table...");
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(TRUNCATE_TBL))
                {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }

        public LinkedList<IAggregateEvent> GetEvents(string aggregateName)
        {
            CreateTableIfRequired();

            // TODO: Entire history until we have snapshots
            var events = new LinkedList<IAggregateEvent>();

            // TODO: improve this...
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(string.Format(SELECT_ALL_EVENTS, aggregateName)))
                {
                    command.Connection = connection;
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var type = Type.GetType(reader["EventType"].ToString());
                        var data = reader["EventData"].ToString();

                        var deserialisedEvent = serialiser.DeSerialiseAs(type, data) as IAggregateEvent;
                        events.AddLast(deserialisedEvent);
                    }

                    reader.Close();
                }
            }

            return events;
        }

        public void SaveEvents(IAggregateRoot aggregateRoot)
        {
            CreateTableIfRequired();

            var changes = aggregateRoot.GetAllChanges();

            var enumerator = changes.GetEnumerator();
            var count=0;
            while (enumerator.MoveNext())
            {
                count++;

                var @event = enumerator.Current;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var data = serialiser.SerialiseAs(@event.GetType(), @event);

                    using (var command = new SqlCommand(SAVE_EVENT))
                    {
                        // eventId - Identity field.
                        command.Parameters.AddWithValue("@AggregateId", @event.AggregateId);
                        command.Parameters.AddWithValue("@EventType", @event.GetType().AssemblyQualifiedName);
                        command.Parameters.AddWithValue("@EventData", data);
                        command.Parameters.AddWithValue("@TimeStamp", @event.TimeStamp);

                        command.Connection = connection;

                        // DumpCmd(command);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    connection.Close();
                }
            }

            logger.DebugFormat("Saved {0} events for {1}.", count, aggregateRoot.EntityName);

            aggregateRoot.MarkCommitted(); 
        }

        private void DumpCmd(SqlCommand cmd)
        {
            logger.DebugFormat(cmd.Parameters.Cast<SqlParameter>().Aggregate(cmd.CommandText, (current, p) => current.Replace(p.ParameterName, p.Value.ToString())));
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Domain.Aggregate
{
    public interface IEventToAggregateEventMapper
    {
        LinkedList<IAggregateEvent> Map(List<Core.Event> events);
    }

    public class EventToAggregateEventMapper : IEventToAggregateEventMapper
    {
        private readonly ILogger logger;
        private readonly ISerialiser serialiser;
        private readonly Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();

        public EventToAggregateEventMapper(ILoggerFactory loggerFactory
                                         , ISerialiser serialiser)
        {
            this.serialiser = serialiser;
            logger = loggerFactory.Create();
        }

        public LinkedList<IAggregateEvent> Map(List<Core.Event> events)
        {
            return MapEventsToTypes(events);
        }

        private LinkedList<IAggregateEvent> MapEventsToTypes(List<Core.Event> list)
        {
            var events = new LinkedList<IAggregateEvent>();

            list.ForEach(x =>
            {
                var type = Type.GetType(x.EventType, AssemblyResolver, TypeResolver);

                events.AddLast(serialiser.DeSerialiseAs(type, x.EventData) as IAggregateEvent);
            });

            return events;
        }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null
                    ? Type.GetType(name, false, ignore)
                    : assembly.GetType(name, false, ignore);
        }

        private Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            if (assemblyCache.ContainsKey(assemblyName.FullName))
            {
                return assemblyCache[assemblyName.FullName];
            }
            else
            {
                var matches = Directory.GetFiles(Environment.CurrentDirectory, Path.Combine(assemblyName.FullName, ".dll"));

                if (matches.Count() == 1)
                {
                    return Assembly.LoadFile(matches.Single());
                }

                throw new Exception(string.Format("No singular matches for: {0} ({1})", assemblyName.FullName, matches.Count()));
            }
        }
    }
}
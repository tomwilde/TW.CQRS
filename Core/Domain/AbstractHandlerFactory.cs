using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Domain;

namespace TW.CQRS.Core.Domain
{
    public abstract class AbstractHandlerFactory<T> : IHandlerFactory<T> where T : class
    {
        protected string handlerTypeName;
        public Dictionary<Type, T> handlers;

        protected readonly ILogger logger;

        protected AbstractHandlerFactory(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.Create();
        }

        public Dictionary<Type, T> GetHandlers()
        {
            return handlers;
        }

        public void Initialise(IUnityContainer container)
        {
            handlers = container.Registrations.Where(x => x.RegisteredType.GetInterfaces().Any(y => y == typeof(T)))
                .Where(x => !x.MappedToType.IsAbstract)
                .Select(x => new Tuple<Type, T>(x.MappedToType.GetInterfaces().First(y => y.IsGenericType).GetGenericArguments().ToList().First(), 
                                                container.Resolve(x.MappedToType) as T))
                .ToDictionary(x => x.Item1, x => x.Item2);

            if (handlers.Count == 0)
            {
                logger.Error(string.Format("No {0} registered... :/", typeof(T).Name));
            }
            else
            {
                logger.Info(string.Format("{0} registration complete, found: {1}", handlerTypeName, handlers.Count));
                handlers.Keys.ToList().ForEach(
                    h => logger.Info(string.Format("  '{0}' is registered as a {1}.", h.Name, handlerTypeName)));
            }
        }
    }
}
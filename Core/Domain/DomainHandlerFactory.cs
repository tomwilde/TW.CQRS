using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using TW.CQRS.Core.Interfaces;
using TW.CQRS.Core.Interfaces.Domain;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain
{
    public class DomainHandlerFactory
    {
        protected string handlerTypeName;
        public Dictionary<Type, IDomainMessageHandler<IDomainMessage>> domainHandlers;

        protected readonly ILogger logger;

        public DomainHandlerFactory(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.Create(this.GetType());
        }

        public Dictionary<Type, IDomainHandler<T>> GetHandlers<T>() where  T : IDomainMessage
        {
            return domainHandlers.Where(x => x.Value is T).ToDictionary(x => x.Key, x=> x.Value as IDomainHandler<T>);
        }

        public void Initialise(IUnityContainer container)
        {
            domainHandlers = container.Registrations.Where(x => x.RegisteredType.GetInterfaces().Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == typeof (IDomainMessageHandler<>)) ))
                .Where(x => !x.MappedToType.IsAbstract)
                .Select(x =>
                {
                    var handlerType = x.MappedToType.GetInterfaces(); //.GetGenericTypeDefinition();

                    if (handlerType.Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == typeof (IDomainMessageHandler<>))))
                    {
                        var data = new Tuple<Type, IDomainMessageHandler<IDomainMessage>>(x.MappedToType, container.Resolve(x.MappedToType) as IDomainMessageHandler<IDomainMessage>);
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }).ToDictionary(x => x.Item1, x => x.Item2);

            if (domainHandlers.Count == 0)
            {
                logger.Error("No DomainHandlers registered... :/");
                throw new Exception();
            }
            else
            {
                logger.Info(string.Format("{0} registration complete, found: {1}", handlerTypeName, domainHandlers.Count));
                domainHandlers.Keys.ToList().ForEach(
                    h => logger.Info(string.Format("  '{0}' is registered as a Domain Command.", h.Name)));
            }
        }
    }
}
using System;
using System.Collections.Generic;

using TW.CQRS.Core.Interfaces;
using TW.CQRS.Core.Interfaces.Domain;
using TW.Commons.Interfaces;


namespace TW.CQRS.Core.Domain
{
    // TODO: do we need a queueing mechanism here?  *OR*  Do we let the whole thing run async on the threadpool..?

    public class Bus<T> : IBus<T> where  T : IDomainMessage
    {
        private readonly DomainHandlerFactory domainHandlerFactory;
        protected Dictionary<Type, IDomainHandler<T>> handlers = new Dictionary<Type, IDomainHandler<T>>();
        protected ILogger logger;

        public Bus(ILoggerFactory loggerFactory, DomainHandlerFactory domainHandlerFactory)
        {
            this.domainHandlerFactory = domainHandlerFactory;
            logger = loggerFactory.Create(this.GetType());
        }

        public void Publish(T message) 
        {
            if (handlers.Count == 0)
            {
                handlers = domainHandlerFactory.GetHandlers<T>();                
            }

            // TODO: If one handler fails, what happens in the handlers that succeed?
            
            logger.Info(string.Format("Publishing event of type:{0} content:{1}", message.GetType(), message));

            if (handlers.ContainsKey(typeof (T)))
            {
                // TODO: go fully async
                handlers[typeof (T)].Handle(message);
            }
            else
            {
                throw new DomainHandlerException<T>("Handler not found!");
            }
        }
    }
}
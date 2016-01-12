using System;
using System.Collections.Generic;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Domain;

namespace TW.CQRS.Core.Domain
{
    // TODO: 
    // do we need a queueing mechanism here?  
    // *OR*  
    // do we let the whole thing run async on the threadpool..?
    //
    // singleton? multiple??

    public abstract class AbstractBus<T> where T : class
    {
        protected ILogger logger;

        private readonly IHandlerFactory<T> handlerFactory;
        private Dictionary<Type, T> handlers;

        public Dictionary<Type, T> Handlers
        {
            get { return handlers ?? (handlers = handlerFactory.GetHandlers()); }
        }

        protected AbstractBus(ILoggerFactory loggerFactory, IHandlerFactory<T> handlerFactory)
        {
            this.handlerFactory = handlerFactory;
            logger = loggerFactory.Create();
        }
    }
}
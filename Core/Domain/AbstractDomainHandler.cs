using System;
using System.Linq;
using TW.CQRS.Core.Interfaces.Domain;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain
{
    public abstract class AbstractDomainHandler<T> where T : class, IDomainMessage
    {
        protected ILogger logger;
        public string Name { get; set; }

        protected AbstractDomainHandler(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.Create();

            Name = this.GetType().Name;
        }

        protected void RaiseException<S>(Exception exception, S domainMessage)
        {
            var props = string.Join(",", domainMessage.GetType().GetProperties().Select(p => string.Format("{0}: {1}", p.Name, p.GetValue(domainMessage, null))));
            logger.Error(string.Format("Failed to process: {0} with parameters: {1}", domainMessage.GetType().Name, props));
            throw exception;
        }
    }
}
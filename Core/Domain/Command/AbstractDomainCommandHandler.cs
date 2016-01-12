using System;
using System.Transactions;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Command
{
    /// <summary>
    /// Commands are processed _within the bounded context_
    /// </summary>
    public abstract class AbstractDomainCommandHandler<T> : AbstractDomainHandler<T>, IDomainCommandHandler<T> where T : class, IDomainCommand
    {
        protected AbstractDomainCommandHandler(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public void Handle(IDomainCommand cmd)
        {
            if (cmd is T)
            {
                Handle(cmd as T);
            }
        }
        
        public abstract void Handle(T command);
        
        protected void WithinTransaction(T command, Action<T> action) 
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    action(command);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                RaiseException(ex, command);
            }
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Command
{
    public class DomainCommandBus : AbstractBus<IGenericDomainCommandHandler>, IDomainCommandBus
    {
        public DomainCommandBus(DomainCommandHandlerFactory handlerFactory, ILoggerFactory loggerFactory)
            : base(loggerFactory, handlerFactory)
        {
        }

        /// <summary>
        /// DomainCommands are assumed to succeed, barring infrastructure failures - in which case they'll never get processed.
        /// </summary>
        /// <param name="command"></param>
        public void Publish<S>(S command) where S : IDomainCommand
        {            
            logger.Info(string.Format("Publishing command of type:{0} content:{1}", command.GetType(), command.ToString()));

            if (Handlers.ContainsKey(typeof(S)))
            {
                Task.Run(() => Handlers[typeof(S)].Handle(command));
            }
            else
            {
                throw new DomainCommandBusException<S>("Command Handler not found!");
            }
        }
    }
}

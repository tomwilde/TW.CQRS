using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Command
{
    public class DomainCommandHandlerFactory : AbstractHandlerFactory<IGenericDomainCommandHandler>
    {
        public DomainCommandHandlerFactory(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            handlerTypeName = "Domain Command";
        }
    }
}
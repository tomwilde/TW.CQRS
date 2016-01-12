using TW.CQRS.Core.Domain;
using TW.CQRS.Core.Domain.Command;

namespace EstateAgent.Domain.Commands
{
    public class AddNewPropertyDomainCommand : AbstractDomainCommand
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}

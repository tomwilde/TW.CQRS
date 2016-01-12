namespace TW.CQRS.Core.Interfaces.Domain.Command
{
    public interface IGenericDomainCommandHandler
    {
        string Name { get; set; }
        void Handle(IDomainCommand message);
    }
}
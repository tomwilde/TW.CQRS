namespace TW.CQRS.Core.Interfaces.Domain.Command
{
    public interface IDomainCommandHandler<T> : IGenericDomainCommandHandler where T : IDomainCommand 
    {
        void Handle(T message);
    }
}
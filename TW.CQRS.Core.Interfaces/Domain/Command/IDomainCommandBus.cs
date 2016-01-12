namespace TW.CQRS.Core.Interfaces.Domain.Command
{
    public interface IDomainCommandBus : IBus
    {
        void Publish<T>(T command) where T : IDomainCommand;
    }
}

namespace TW.CQRS.Core.Interfaces.Domain
{
    public interface IDomainMessageHandler<T> where T : IDomainMessage
    {
        string Name { get; set; }
        void Handle(T message);
    }
}
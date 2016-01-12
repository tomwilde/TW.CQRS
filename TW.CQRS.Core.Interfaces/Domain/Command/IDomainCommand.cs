namespace TW.CQRS.Core.Interfaces.Domain.Command
{
    // TODO: Commands are asyncronous, we should enforce this explicitly!!
    public interface IDomainCommand : IDomainMessage 
    {
    }
}
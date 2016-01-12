using System;
using TW.CQRS.Core.Interfaces.Domain;
using TW.CQRS.Core.Interfaces.Domain.Command;

namespace TW.CQRS.Core.Domain.Command
{
    public class DomainCommandBusException<T> : AbstractBusException where T : IDomainCommand
    {
        public DomainCommandBusException(string error) : base(error)
        {
        }

        public DomainCommandBusException(Exception innerException) : base(string.Format("Domain Command Bus Exceptions for message type: {0}", typeof (T)), innerException)
        {
        }
    }
}
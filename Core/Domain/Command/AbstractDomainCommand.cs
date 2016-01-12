using System;
using TW.CQRS.Core.Interfaces.Domain.Command;

namespace TW.CQRS.Core.Domain.Command
{
    public abstract class AbstractDomainCommand : IDomainCommand
    {
        private readonly DateTime timeStamp;

        protected AbstractDomainCommand()
        {
            this.timeStamp = DateTime.Now;
        }

        public int Id { get; private set; }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
        }
    }
}
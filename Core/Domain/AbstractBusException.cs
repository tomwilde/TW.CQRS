using System;
using System.Collections.Generic;

namespace TW.CQRS.Core.Domain
{
    public abstract class AbstractBusException : Exception
    {
        public string message { get; set; }
        public Exception BusException { get; set; }

        protected AbstractBusException(string message, Exception innerException) : base(message)
        {
            BusException = innerException;
        }

        protected AbstractBusException(string message)
        {
            this.message = message;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using TW.CQRS.Core.Interfaces;
using TW.CQRS.Core.Interfaces.Domain;
using TW.Commons;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain
{
    public class DomainHandlerException<T> : Exception
    {
        private readonly string info;

        public DomainHandlerException(string info)
        {
            this.info = info;
        }

        public string Info
        {
            get { return info; }
        }
    }
}
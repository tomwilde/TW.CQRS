using System;
using System.Collections.Generic;

namespace TW.CQRS.Core.Interfaces.Domain
{
    public interface IHandlerFactory<T> where T : class
    {
        Dictionary<Type, T> GetHandlers();
    }
}
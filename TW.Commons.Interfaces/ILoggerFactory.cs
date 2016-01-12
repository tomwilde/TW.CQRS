using System;

namespace TW.Commons.Interfaces
{
    ///<summary>
    /// Dependency Injectable Logger Factory
    ///</summary>
    public interface ILoggerFactory
    {
        ILogger Create();
    }
}

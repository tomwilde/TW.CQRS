using System;
using System.Diagnostics;
using TW.Commons.Interfaces;
using log4net;

namespace TW.Commons.Logging
{
    /// <summary>
    /// Dependency Injectable Logger Factory.
    /// </summary>
    public class Log4netLoggerFactory : ILoggerFactory
    {
        private static readonly object lockr = new object();
        private static bool configured = false;

        public virtual ILogger Create(Type type)
        {
            // Dev note: - double lock check
            if (!configured)
            {
                lock(lockr)
                { 
                    if (!configured)
                    {                      
                        // Get config from app config
                        log4net.Config.XmlConfigurator.Configure();
                        configured = true;
                    }
                }
            }

            return new Log4netLogger(LogManager.GetLogger(type));
        }

        /// <summary>
        /// Gets the logger for the given type (uses StackFrame to get the calling type)
        /// </summary>
        /// <returns></returns>
        public ILogger Create()
        {
            var callingType = new StackFrame(1, false).GetMethod().DeclaringType;

            return Create(callingType);
        }
    }
}
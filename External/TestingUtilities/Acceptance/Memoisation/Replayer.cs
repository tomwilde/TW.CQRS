using System;
using System.Linq;
using System.Reflection;
using TW.Commons.Interfaces;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    public class Replayer<T>
    {
        private readonly dynamic _generator = WorkAroundStupidHardLinked_MarkedInternalLibs_GetProxyGenerator();

        private readonly ILoggerFactory _loggerFactory;
        private readonly string _storagePath;
        private readonly ILogger _logger;
        
        public Replayer(ILoggerFactory loggerFactory, string storagePath)
        {
            _loggerFactory = loggerFactory;
            _storagePath = storagePath;

            _logger = loggerFactory.Create();
            _logger.InfoFormat("Replaying<{0}>", typeof(T));
        }

        public T Object
        {
            get
            {
                Type type = _generator.GetType();
                var list = type.GetMembers().Where(x => x.Name.Contains("CreateInterfaceProxyWithoutTarget")).ToList();

                var iHateStaticallyLinkedLibraries = (list[3] as MethodInfo);

                return iHateStaticallyLinkedLibraries.Invoke(_generator, new object[]
                {
                    typeof(T), 
                    new Castle.Core.Interceptor.IInterceptor[]
                    {
                        // TODO: handle Props
                        new MockDataReplayInterceptor<T>(_loggerFactory, _storagePath)
                    }
                });



                //return _generator.CreateInterfaceProxyWithoutTarget<T>(new RMCastle.IInterceptor[]
                //{
                //    // TODO: handle Props
                //    new MockDataReplayInterceptor<T>(_loggerFactory, _pathToReplayFiles)
                //});
            }
        }

        private static dynamic WorkAroundStupidHardLinked_MarkedInternalLibs_GetProxyGenerator()
        {
            var type = typeof(Replayer<>).Assembly.GetType("Castle.DynamicProxy.ProxyGenerator");

            return Activator.CreateInstance(type);
        }
    }
}
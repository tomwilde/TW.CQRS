using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using TW.Commons.Interfaces;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    /// <summary>
    /// Memoises in/output to/from a svc, use with known processes to capture test data.
    /// usage: var svc = new Memoiser&lt;FooServiceOfStrings, IFooService&lt;string&gt;&gt;(new FooServiceOfStrings(), o => serialiser.Serialise(o.GetType(), o));
    /// </summary>
    /// <typeparam name="T">T : interface</typeparam>
    public class Memoiser<T> 
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private readonly T _instance;
        private readonly dynamic _generator = WorkAroundStupidHardLinked_MarkedInternalLibs_GetProxyGenerator("Castle.DynamicProxy.ProxyGenerator");

        public string StoragePath { get; set; }

        public Memoiser(ILoggerFactory loggerFactory, IUnityContainer container, string storagePath = null)
        {
            _loggerFactory = loggerFactory;
            StoragePath = storagePath ?? Path.Combine(Assembly.GetEntryAssembly().Location, "Memoisation");

            _logger = loggerFactory.Create();
            _logger.InfoFormat("Memoising<{0}>", typeof(T));

            _instance = container.Resolve<T>();
        }

        private static dynamic WorkAroundStupidHardLinked_MarkedInternalLibs_GetProxyGenerator(string typeName)
        {
            return Activator.CreateInstance(typeof(Memoiser<>).Assembly.GetType(typeName));
        }

        public T Object
        {
            get
            {
                _logger.InfoFormat("Memoising service requested: {0}", typeof(T));

                EnsureDirectoryExists(StoragePath);

                Type type = _generator.GetType();
                var list = type.GetMembers().Where(x => x.Name.Contains("CreateInterfaceProxyWithTarget")).ToList();

                var iHateStaticallyLinkedLibraries = (list[2] as MethodInfo);

                return iHateStaticallyLinkedLibraries.Invoke(_generator, new object[]
                {
                    typeof(T), 
                    _instance, 
                    new Castle.Core.Interceptor.IInterceptor[]
                    {
                        // TODO: handle Props
                        new MockDataCaptureInterceptor(_loggerFactory, typeof(T), StoragePath)
                    }
                });
            }
        }
        
        public string EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = Path.GetDirectoryName(path);
            }

            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            _logger.InfoFormat("Using storage direrctory @ {0}", path);

            return path;
        }
    }
}
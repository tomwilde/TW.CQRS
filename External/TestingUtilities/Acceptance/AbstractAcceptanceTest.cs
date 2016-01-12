using System.Linq;
using Castle.Core.Interceptor;
using Microsoft.Practices.Unity;
using TW.Commons.Interfaces;
using TW.Commons.TestingUtilities.Acceptance.Memoisation;
using TW.Commons.TestingUtilities.Acceptance.Scenarios;

namespace TW.Commons.TestingUtilities.Acceptance
{
    public abstract class AbstractAcceptanceTest<U>
    {
        private readonly ILoggerFactory _loggerFactory;
        protected ILogger _logger;

        protected AbstractAcceptanceTest(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.Create();
        }

        protected Scenario<U> Scenario(U sut)
        {
            return new Scenario<U>(_loggerFactory, sut);
        }

        /// <summary>
        /// Use this to replay your captured memoised data in sequence...
        /// </summary>
        /// <param name="container"></param>
        /// <param name="storagePath"></param>
        /// <typeparam name="S">Interface</typeparam>
        protected virtual AbstractAcceptanceTest<U> RegisterServiceForReplay<S>(UnityContainer container, string storagePath)
        {
            var instance = new Replayer<S>(_loggerFactory, storagePath);
            container.RegisterInstance(instance.Object);
            return this;
        }

        /// <summary>
        /// Use this method to provide default responses, ie: for a newly developed method running against an old recording
        /// </summary>
        public void AddDefaultImplementation<S,R>(UnityContainer container, string methodToOverride, R returnValue)
        {
            var obj = container.Resolve<S>();
            var interceptors = obj.GetType().GetField("__interceptors");
            var interceptor = (interceptors.GetValue(obj) as IInterceptor[]).First();

            (interceptor as MockDataReplayInterceptor<S>).RegisterDefaultImplementation(methodToOverride, returnValue);
        }

        protected virtual UnityContainer BuildContainer()
        {
            return new UnityContainer();
        }

        protected virtual U BuildSUT(UnityContainer container)
        {
            return container.Resolve<U>();
        }
    }
}
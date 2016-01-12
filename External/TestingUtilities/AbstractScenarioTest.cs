using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;

namespace TW.Commons.TestingUtilities
{
    public abstract class AbstractScenarioTest<T> where T : IDisposable
    {
        public T BuildSUT()
        {
            var container = new UnityContainer();

            SetupDependencies(container);

            container.RegisterType<T>();

            return container.Resolve<T>();
        }

        protected abstract void SetupDependencies(IUnityContainer container);

        public void WaitUntil(Func<bool> condition, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();

            while (!condition())
            {
                Thread.Sleep(500);
                if ((sw.Elapsed - timeout).TotalSeconds > 0) throw new TimeoutException("condition was not met within timeout.");
                Console.WriteLine("Waiting for condition to match");
            }
            Console.WriteLine("Condition met.");
        }
    }
}
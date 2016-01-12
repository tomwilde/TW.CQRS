using Microsoft.Practices.Unity;
using Moq;

namespace TW.Commons.TestingUtilities.AutoMoqMocking
{
    public static class MoqAutoMockingExtensions
    {
        public static Mock<T> RegisterMock<T>(this IUnityContainer container) where T : class
        {
            var mock = new Mock<T>();

            container.RegisterInstance<Mock<T>>(mock, new ContainerControlledLifetimeManager());
            container.RegisterInstance<T>(mock.Object, new ContainerControlledLifetimeManager());

            return mock;
        }

        /// <summary>
        /// Use this to add additional setups for a mock that is already registered
        /// </summary>
        public static Mock<T> ConfigureMockFor<T>(this IUnityContainer container) where T : class
        {
            return container.Resolve<Mock<T>>();
        }

        public static void VerifyMockFor<T>(this IUnityContainer container) where T : class
        {
            container.Resolve<Mock<T>>().VerifyAll();
        }
    }
}
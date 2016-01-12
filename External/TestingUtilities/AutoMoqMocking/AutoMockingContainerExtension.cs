using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace TW.Commons.TestingUtilities.AutoMoqMocking
{
    public class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly AutoMockingBuilderStrategy _strategy;

        public AutoMockingContainerExtension(AutoMockingBuilderStrategy strategy)
        {
            _strategy = strategy;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(_strategy, UnityBuildStage.PreCreation);
        }
    }
}
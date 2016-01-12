namespace TW.Commons.TestingUtilities.AutoMoqMocking
{
    public abstract class AbstractAutoMockingTest<T> where T : class
    {
        protected virtual T BuildSUT(AutoMockContainer container)
        {
            return container.Resolve<T>();
        }

        protected virtual AutoMockContainer BuildContainer()
        {
            var container = new AutoMockContainer();

            container.RegisterConcreteType<T>();

            SetDependencies(container);

            return container;
        }

        public abstract void SetDependencies(AutoMockContainer container);
    }
}

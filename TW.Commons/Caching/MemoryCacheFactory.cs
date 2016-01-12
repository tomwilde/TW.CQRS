namespace TW.CQRS.Core.Interfaces
{
    public class MemoryCacheFactory : IMemoryCacheFactory
    {
        public IMemoryCache<T> Get<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}
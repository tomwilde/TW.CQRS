namespace TW.CQRS.Core.Interfaces
{
    public class MemoryCache<T> : IMemoryCache<T>
    {
        public void Set(string key, T data)
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet(string key, out T item)
        {
            throw new System.NotImplementedException();
        }

        public T Get(string key)
        {
            throw new System.NotImplementedException();
        }

        public T this[string key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public void Remove(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}
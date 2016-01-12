using System;
using System.Collections.Generic;
using System.Linq;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.Memory
{
    public class MemoryReportingRepository : IReportingRepository
    {
        #region Internal
        internal class CacheData
        {
            public CacheData(IReportingEntity entity)
            {
                Data = entity;
            }

            public IReportingEntity Data { get; set; }

            public int Id
            {
                get { return Data.Id; }
            }
        }

        internal class CacheDictionary : Dictionary<Type, Dictionary<int, CacheData>>
        {
            
        }
        #endregion

        private readonly ILogger _logger;

        private readonly object lockr = new object();

        private readonly CacheDictionary _cache = new CacheDictionary();

        public MemoryReportingRepository(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create();
        }

        public void Add<T>(T entity) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                var type = typeof (T);
                var data = new CacheData(entity);

                if (_cache.ContainsKey(type))
                {
                    var newId = _cache[type].Max(x => x.Key) + 1;
                    _cache[type].Add(newId, data);
                }
                else
                {
                    _cache.Add(type, new Dictionary<int, CacheData>() {{1, data}});
                }
            }
        }

        public List<T> GetAll<T>() where T : class, IReportingEntity
        {
            lock (lockr)
            {
                var type = typeof (T);
                if (_cache.ContainsKey(type))
                {
                    return _cache[type].Values.Select(x => x.Data.Clone()).Cast<T>().ToList();
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public List<T> Get<T>(Func<IQueryable<T>, List<T>> predicate) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                var type = typeof (T);
                if (_cache.ContainsKey(type))
                {
                    return predicate(_cache[type].Values.Select(x => x.Data.Clone()).Cast<T>().AsQueryable());
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public void Update<T>(T entity) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                var type = typeof (T);
                var entityData = new CacheData(entity);

                if (_cache.ContainsKey(type))
                {
                    if (!_cache[type].ContainsKey(entityData.Data.Id))
                    {
                        throw new Exception(string.Format("Entity not present! {0}", entityData.Data.Id));
                    }

                    _cache[type][entityData.Data.Id] = entityData;
                }
                else
                {
                    _cache.Add(type, new Dictionary<int, CacheData>() {{entityData.Id, entityData}});
                }
            }
        }

        public void Update<T>(List<T> entities) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                entities.ForEach(Update);
            }
        }

        public void Delete<T>(T entity) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                var type = typeof (T);

                if (_cache.ContainsKey(type))
                    _cache.Remove(type);
                else
                    throw new Exception(string.Format("Missing type for Deletion? {0}", type));
            }
        }

        public void Delete<T>(List<T> entities) where T : class, IReportingEntity
        {
            lock (lockr)
            {
                entities.ForEach(Delete);
            }
        }
    }
}
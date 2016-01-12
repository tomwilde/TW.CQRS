using System;
using System.Collections.Generic;
using System.Linq;
using TW.Commons.Data.NHibernate;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.NHib
{
    public class NHibReportingRepository : IReportingRepository 
    {
        private readonly IGenericEntityService _genericEntityService;

        public NHibReportingRepository(IGenericEntityService genericEntityService)
        {
            _genericEntityService = genericEntityService;
        }

        public void Add<T>(T entity) where T : class, IReportingEntity
        {
            _genericEntityService.SaveOrUpdateEntity(entity);
        }

        public List<T> GetAll<T>() where T : class, IReportingEntity
        {
            return _genericEntityService.GetAll<T>();
        }

        public List<T> Get<T>(Func<IQueryable<T>, List<T>> predicate) where T : class, IReportingEntity
        {
            return _genericEntityService.Get(predicate);
        }

        public void Update<T>(T entity) where T : class, IReportingEntity
        {
            _genericEntityService.SaveOrUpdateEntity(entity);
        }

        public void Update<T>(List<T> entities) where T : class, IReportingEntity
        {
            _genericEntityService.SaveOrUpdateEntities(entities);
        }

        public void Delete<T>(T entity) where T : class, IReportingEntity
        {
            _genericEntityService.DeleteEntity(entity);
        }

        public void Delete<T>(List<T> entities) where T : class, IReportingEntity
        {
            _genericEntityService.DeleteEntities(entities);
        }
    }
}
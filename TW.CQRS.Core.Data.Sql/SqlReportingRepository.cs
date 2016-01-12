using System;
using System.Collections.Generic;
using System.Linq;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.Sql
{
    public class SqlReportingRepository : IReportingRepository
    {
        public void Add<T>(T entity) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<T>() where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public List<T> Get<T>(Func<IQueryable<T>, List<T>> predicate) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T entity) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public void Update<T>(List<T> entities) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(List<T> entities) where T : class, IReportingEntity
        {
            throw new NotImplementedException();
        }
    }
}
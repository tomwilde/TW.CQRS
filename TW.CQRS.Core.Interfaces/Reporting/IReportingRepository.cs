using System;
using System.Collections.Generic;
using System.Linq;

namespace TW.CQRS.Core.Interfaces.Reporting
{
    public interface IReportingRepository
    {
        // C
        void Add<T>(T entity) where T : class, IReportingEntity;

        // R
        List<T> GetAll<T>() where T : class, IReportingEntity;
        List<T> Get<T>(Func<IQueryable<T>, List<T>> predicate) where T : class, IReportingEntity;

        // U
        void Update<T>(T entity) where T : class, IReportingEntity;
        void Update<T>(List<T> entities) where T : class, IReportingEntity;

        // D
        void Delete<T>(T entity) where T : class, IReportingEntity;
        void Delete<T>(List<T> entities) where T : class, IReportingEntity;
    }
}
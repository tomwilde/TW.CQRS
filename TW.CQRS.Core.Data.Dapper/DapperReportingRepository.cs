using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Data.Dapper
{
    public class DapperReportingRepository : IReportingRepository 
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DapperReportingRepository(ILoggerFactory loggerFactory
                                       , IConnectionStringProvider connectionStringProvider)
        {
            _connectionString = connectionStringProvider.Get("events");
            _logger = loggerFactory.Create();
        }

        public void Add<T>(T entity) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public List<T> GetAll<T>() where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public List<T> Get<T>(Func<IQueryable<T>, List<T>> predicate) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public void Update<T>(T entity) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public void Update<T>(List<T> entities) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public void Delete<T>(T entity) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }

        public void Delete<T>(List<T> entities) where T : class, IReportingEntity
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                throw new NotImplementedException();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using TW.Commons.Interfaces;
using ILoggerFactory = TW.Commons.Interfaces.ILoggerFactory;

namespace TW.Commons.Data.NHibernate
{
    public class GenericEntityService : IGenericEntityService
    {
        private readonly ISessionFactory _sessionFactory;
        private const int standardProcTimeout = 600;
        private readonly ILogger _logger;

        public GenericEntityService(ILoggerFactory loggerFactory, ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _logger = loggerFactory.Create();
        }

        #region Get Related
        /// <summary>
        /// Perform a custom query using Linq (returns same type as that queried as List<>)
        /// </summary>
        public List<T> Get<T>(Func<IQueryable<T>, List<T>> query, bool stateless = true) where T : class
        {
            return Get(query, stateless);
        }

        /// <summary>
        /// Perform a custom query using Linq, projecting the results to a result type other than that queried.
        /// </summary>
        public TReturn Get<TQuery, TReturn>(Func<IQueryable<TQuery>, TReturn> query, bool stateless = true) where TQuery : class
        {
            TReturn result;

            if(stateless)
            {
                using (var session = _sessionFactory.OpenStatelessSession())
                {
                    result = query.Invoke(session.Query<TQuery>());
                }
            }
            else
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    result = query.Invoke(session.Query<TQuery>());
                }
            }

            return result;
        }

        public List<T> Get<T>(Func<IQueryable<T>, List<T>> query, IInterceptor queryInterceptor, bool stateless = true) where T : class
        {
            return Get<T, List<T>>(query,queryInterceptor,stateless);
        }

        public TReturn Get<TQuery, TReturn>(Func<IQueryable<TQuery>, TReturn> query, IInterceptor queryInterceptor, bool stateless = true) where TQuery : class
        {
            TReturn result;
            
            if (stateless)
            {
                using (var session = _sessionFactory.OpenStatelessSession())
                {
                    result = query.Invoke(session.Query<TQuery>());
                }
            }
            else
            {
                using (var session = _sessionFactory.OpenSession(queryInterceptor))
                {
                    result = query.Invoke(session.Query<TQuery>());
                }
            }

            return result;
        }


        /// <summary>
        /// Gets all entities in the list
        /// </summary>
        /// <typeparam name="T">Given Entity object needs to have Session attribute. see <see cref="UBS.FIT.Rita.Common.Core.Attributes.SessionTypeAttribute"/></typeparam>
        /// <param name="stateless"></param>
        /// <param name="query"> </param>
        /// <param name="sessionType">If your entity is used across multiple DBs, you might specify it explicitly. 
        /// When set to none, it will try to derive from Entity's attribute</param>
        /// <returns></returns>
        public List<T> GetAll<T>(bool stateless = true, IQueryable<T> query = null) where T : class
        {
            if(stateless)
            {
                using (var session = _sessionFactory.OpenStatelessSession())
                {
                    if(query!=null)
                    {
                        return query.ToList();
                    }

                    var result = Enumerable.ToList<T>(session.Query<T>());
                    return result;
                }
            }
            else
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    if(query!=null)
                    {
                        return query.ToList();
                    }

                    var result = Enumerable.ToList<T>(session.Query<T>());
                    return result;
                }
            }
        }
        /// EVITA-229 : Removing session timeout setting, this is now set through the Hibernate command timeout.
        /// Use App configuration setting HibernateDatabaseCommandTimeoutSeconds to set the timeout.
        /// <summary>
        /// Executes the given stored proc and auto maps the result down to the specified entity.
        /// Query must be supplied in the form: "exec " + procName + " :@argname1,:@argname2"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureWithNamedParameters"></param>
        /// <param name="parameterConfiguration"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public List<T> GetUsingStoredProcedure<T>(string procedureWithNamedParameters, Func<ISQLQuery, IQuery> parameterConfiguration, 
                                                  int timeoutInSeconds = -1)
        {
            var result = new List<T>();

            if (timeoutInSeconds == -1) timeoutInSeconds = standardProcTimeout;

            GetSessionAndExecute<T>(session => result.AddRange(parameterConfiguration(session.CreateSQLQuery("exec " + procedureWithNamedParameters))
                                                                 .SetResultTransformer(Transformers.AliasToBean<T>()).List<T>()));
            return result;
        }

        #endregion

        #region Save/Update Related

        /// <summary>
        /// Saves or Updates the given Entity
        /// </summary>
        public void SaveOrUpdateEntity<T>(T entity) where T : class
        {
            GetSessionAnExecuteWithinTransaction<T>(session => session.SaveOrUpdate(entity));
        }

        /// <summary>
        /// Saves or Updates the given Entities
        /// </summary>
        public void SaveOrUpdateEntities<T>(IEnumerable<T> entities) where T : class
        {
            try
            {
                GetSessionAnExecuteWithinTransaction<T>(session => entities.ForEach(session.SaveOrUpdate));
            }
            catch(ADOException ex)
            {
                if(ex.InnerException.Message.Contains("UNIQUE KEY constraint"))
                {
                    throw new Exception("A database rule has been broken:\r\n" + ex.InnerException.Message);
                }                
                throw;                
            }
        }

        #endregion

        #region Delete Related

        /// <summary>
        /// Deletes the Entity
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity<T>(T entity) where T : class
        {
            GetSessionAnExecuteWithinTransaction<T>(session => session.Delete(entity));
        }

        /// <summary>
        /// Deletes the Entity
        /// </summary>
        /// <param name="entities"></param>
        public void DeleteEntities<T>(IEnumerable<T> entities) where T : class
        {
            GetSessionAnExecuteWithinTransaction<T>(session => entities.ForEach(session.Delete));
        } 

        #endregion

        protected void GetSessionAndExecute<T>(Action<ISession> action)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                action(session);
                session.Flush();
            }
        }

        protected void GetStatelessSessionAndExecute<T>(Action<IStatelessSession> action)
        {
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                action(session);
            }
        }

        protected void GetSessionAnExecuteWithinTransaction<T>(Action<ISession> action)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    action(session);

                    session.Flush();
                    transaction.Commit();
                }
            }
        }
    }
}
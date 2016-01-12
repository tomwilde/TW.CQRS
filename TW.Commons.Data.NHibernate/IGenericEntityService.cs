using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace TW.Commons.Data.NHibernate
{
    public interface IGenericEntityService
    {
        /// <summary>
        /// Perform a custom query using Linq (returns same type as that queried as List<>)
        /// </summary>
        List<T> Get<T>(Func<IQueryable<T>, List<T>> query, bool stateless = true) where T : class;

        /// <summary>
        /// Perform a custom query using Linq, projecting the results to a result type other than that queried.
        /// </summary>
        TReturn Get<TQuery, TReturn>(Func<IQueryable<TQuery>, TReturn> query, bool stateless = true) where TQuery : class;

        List<T> Get<T>(Func<IQueryable<T>, List<T>> query, IInterceptor queryInterceptor, bool stateless = true) where T : class;
        TReturn Get<TQuery, TReturn>(Func<IQueryable<TQuery>, TReturn> query, IInterceptor queryInterceptor, bool stateless = true) where TQuery : class;

        /// <summary>
        /// Gets all entities in the list
        /// </summary>
        /// <typeparam name="T">Given Entity object needs to have Session attribute. see <see cref="UBS.FIT.Rita.Common.Core.Attributes.SessionTypeAttribute"/></typeparam>
        /// <param name="stateless"></param>
        /// <param name="query"> </param>
        /// <param name="sessionType">If your entity is used across multiple DBs, you might specify it explicitly. 
        /// When set to none, it will try to derive from Entity's attribute</param>
        /// <returns></returns>
        List<T> GetAll<T>(bool stateless = true, IQueryable<T> query = null) where T : class;

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
        List<T> GetUsingStoredProcedure<T>(string procedureWithNamedParameters, Func<ISQLQuery, IQuery> parameterConfiguration, 
            int timeoutInSeconds = -1);

        /// <summary>
        /// Saves or Updates the given Entity
        /// </summary>
        void SaveOrUpdateEntity<T>(T entity) where T : class;

        /// <summary>
        /// Saves or Updates the given Entities
        /// </summary>
        void SaveOrUpdateEntities<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// Deletes the Entity
        /// </summary>
        /// <param name="entity"></param>
        void DeleteEntity<T>(T entity) where T : class;

        /// <summary>
        /// Deletes the Entity
        /// </summary>
        /// <param name="entities"></param>
        void DeleteEntities<T>(IEnumerable<T> entities) where T : class;
    }
}
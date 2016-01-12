using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using TW.CQRS.Core.Interfaces.Reporting;
using log4net;

namespace TW.CQRS.Core.Reporting
{
    public class ReportingRepository : IRepository
    {
        private readonly ILog logger;

        public ReportingRepository()
        {
            logger = LogManager.GetLogger(this.GetType());
        }

        public T Get<T>(Guid id) where T : class, new()
        {
            return SessionFunc<T>(sessionFactory.OpenSession(), s => s.Get<T>(id));
        }

        // TODO: decide what to do about this...
        public List<T> Get<T>(ICriteria criteria) where T : class
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<T>() where T : class, new()
        {
            return SessionFunc(sessionFactory.OpenSession(), s => s.CreateCriteria<T>().List<T>().ToList());
        }
        
        private T SessionFunc<T>(ISession session, Func<ISession, T> func) where T : class, new()
        {
            var result = default(T);

            try
            {
                result = func.Invoke(session);
                session.Flush();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                session.Close();
                session.Dispose();
            }

            return result;
        }

        private void SessionAction(ISession session, Action<ISession> action)
        {
            try
            {
                action.Invoke(session);
                session.Flush();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                session.Close();
                session.Dispose();
            }
        }    
    }
}
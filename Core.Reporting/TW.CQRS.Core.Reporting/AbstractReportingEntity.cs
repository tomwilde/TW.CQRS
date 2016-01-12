using System;

namespace TW.CQRS.Core.Reporting
{
    /// <summary>
    /// NHibernate is the assumed store for the reporting interface
    /// </summary>
    public abstract class AbstractReportingEntity
    {
        public virtual Guid Id { get; private set; }
    }
}

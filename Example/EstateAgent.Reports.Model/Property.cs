using System;
using TW.CQRS.Core.Interfaces.Reporting;

namespace EstateAgent.Reports.Model
{
    /// <summary>
    /// Currently available properties... 
    /// </summary>
    public class Property : IReportingEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
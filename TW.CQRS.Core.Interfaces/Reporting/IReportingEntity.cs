using System;

namespace TW.CQRS.Core.Interfaces.Reporting
{
    public interface IReportingEntity : ICloneable
    {
        int Id { get; set; }
    }
}
using System;
using TW.Commons.Events;

namespace TW.CQRS.Core.Interfaces.Reporting
{
    public interface IReportingUpdatesBus
    {
        event EventHandler<EventArgs<string>> Update;

        void Publish(string name);
    }
}
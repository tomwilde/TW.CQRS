using System;
using TW.Commons.Events;
using TW.Commons.Interfaces;
using TW.CQRS.Core.Interfaces.Reporting;

namespace TW.CQRS.Core.Domain.Reporting
{
    public class ReportingUpdatesBus : IReportingUpdatesBus
    {
        private readonly ILogger _logger;

        public event EventHandler<EventArgs<string>> Update;

        public ReportingUpdatesBus(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create();
        }

        public void Publish(string name)
        {
            if (Update != null)
            {
                _logger.DebugFormat("Raising Reporting Update event: {0}", name);
                Update(this, new EventArgs<string>(name));
            }
        }
    }
}
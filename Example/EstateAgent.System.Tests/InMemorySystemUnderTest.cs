using System;
using System.Reactive.Linq;
using TW.Commons.Events;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.CQRS.Core.Interfaces.Reporting;

namespace EstateAgent.System.Tests
{
    /// <summary>
    /// Encapsulates both Read & Write elements of the WHOLE system.
    /// => Tests run within memory
    /// </summary>
    public class InMemorySystemUnderTest : IDisposable
    {
        private readonly IDomainCommandBus _commandBus;
        private readonly IReportingUpdatesBus _reportingUpdatesBus;

        public IReportingRepository ReportingRepository { get; private set; }

        public InMemorySystemUnderTest(IDomainCommandBus commandBus
                                     , IReportingUpdatesBus reportingUpdatesBus                         
                                     , IReportingRepository reportingRepository)
        {
            _commandBus = commandBus;

            _reportingUpdatesBus = reportingUpdatesBus;
            
            ReportingRepository = reportingRepository;
        }

        public void IssueDomainCommand<T>(T cmd) where T : IDomainCommand
        {
            _commandBus.Publish(cmd);
        }

        public IObservable<string> SubscribeToUpdates()
        {
            return Observable.FromEventPattern<EventArgs<string>>(_reportingUpdatesBus, "Update").Select(x => x.EventArgs.EventData);
        }

        public void Dispose()
        {
            // foo
        }
    }
}

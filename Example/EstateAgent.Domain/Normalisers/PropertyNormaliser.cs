using System;
using System.Threading;
using System.Threading.Tasks;
using TW.CQRS.Core.Interfaces;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.CQRS.Core.Interfaces.Reporting;
using TW.Commons.Interfaces;

using EstateAgent.Domain.Events;
using EstateAgent.Reports.Model;


namespace EstateAgent.Domain.Normalisers
{
    public class EstateAgentNormaliser : INormaliser
    {
        private readonly IReportingRepository _reportingRepository;
        private readonly IReportingUpdatesBus _reportingUpdatesBus;
        private readonly ILogger _logger;

        public EstateAgentNormaliser(ILoggerFactory loggerFactory
                                   , IReportingRepository reportingRepository
                                   , IReportingUpdatesBus reportingUpdatesBus)
        {
            _reportingRepository = reportingRepository;
            _reportingUpdatesBus = reportingUpdatesBus;
            _logger = loggerFactory.Create();
        }

        public void Normalise(IDomainEvent @event)
        {
            _logger.Info("Normalising...");

            // dispatch accordingly
            if (Dispatch<PropertyRegisteredDomainEvent>(@event, HandlePropertyRegisteredDomainEvent)) return;

            _logger.WarnFormat("Couldnt handle event of id/type: {0}/{1} - nothing configured.");
        }

        private bool Dispatch<T>(IDomainEvent @event, Action<IDomainEvent> func) where T : class
        {
            if (@event is T)
            {
                func(@event);
                return true;
            }
            return false;
        }

        private void HandlePropertyRegisteredDomainEvent(IDomainEvent @event)
        {
            if (@event is PropertyRegisteredDomainEvent)
            {
                var ev = @event as PropertyRegisteredDomainEvent;
                
                _reportingRepository.Add(new Property()
                {
                    Id = @ev.PropertyId,
                    Name = @ev.Name,
                    Price = @ev.Price
                });

                _logger.InfoFormat("Normalising: New Property added! {0}", @ev.PropertyId);
                
                // pass this onto the UI, whatever...
                Task.Run(() => _reportingUpdatesBus.Publish(ReportingUpdateType.Property));
            }
            else
            {
                var type = @event == null ? null : @event.GetType();
                _logger.WarnFormat("Normalising: failed, event was not the expected type! {0}", type);
            }
        }
    }
}
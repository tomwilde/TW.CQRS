using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EstateAgent.Common;
using EstateAgent.Domain.Commands;
using EstateAgent.Reports.Model;
using TW.Commons.Events;
using TW.CQRS.Core.Interfaces.Domain.Command;
using TW.CQRS.Core.Interfaces.Reporting;


namespace EstateAgent.ViewModels
{
    public class DashBoardViewModel : AbstractViewModel
    {
        private const string Name = "Dashboard";

        private readonly IReportingRepository _reportingRepository;
        private readonly IDomainCommandBus domainCommandBus;

        private List<Property> properties = new List<Property>();

        private string newName;
        private decimal newPrice;

        public DelegateCommand Add { get; set; }

        public DashBoardViewModel(IDomainCommandBus domainCommandBus
                                , IReportingRepository reportingRepository
                                , IReportingUpdatesBus reportingUpdatesBus)
            : base(Name)
        {
            _reportingRepository = reportingRepository;

            this.domainCommandBus = domainCommandBus;

            this.Add = new DelegateCommand(o => 
                {
                    this.domainCommandBus.Publish(new AddNewPropertyDomainCommand()
                    {
                        Name = this.newName,
                        Price = this.newPrice
                    });

                    this.NewName = string.Empty;
                    this.newPrice = 0.0m;
                });

            Observable.FromEventPattern<EventArgs<string>>(reportingUpdatesBus, "Update")
                .Subscribe(pattern => 
                    Dispatcher.BeginInvoke(new Action(RefreshData)));
        }
        
        private void RefreshData()
        {
            logger.Debug("Getting latest data.");
            Properties = _reportingRepository.GetAll<Property>().ToList();
        }

        public List<Property> Properties
        {
            get { return this.properties; }
            set
            {
                this.properties = value;
                RaisePropertyChangedEvent(() => this.Properties);
                RaisePropertyChangedEvent(() => this.PropertyCount);
            }
        }

        public int PropertyCount
        {
            get { return this.properties.Count; }
        }
        
        public string NewName
        {
            get { return this.newName; }
            set
            {
                this.newName = value;
                RaisePropertyChangedEvent(() => this.NewName);
            }
        }

        public decimal NewPrice
        {
            get { return this.newPrice; }
            set
            {
                this.newPrice = value;
                RaisePropertyChangedEvent(() => this.NewPrice);
            }
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using System.Windows;

using Microsoft.Practices.Unity;
// Bzzzt

using TW.CQRS.Core.Domain.Command;
using TW.CQRS.Core.Domain.Event;

using EstateAgent.Reports.Model;
using EstateAgent.ViewModels;
using EstateAgent.Views;


namespace EstateAgent
{
    public class BootStrapper
    {
        private static readonly IUnityContainer container = new UnityContainer();

        public void Run()
        {
            ConfigureContainer();
        }

        public UIElement ShowMainShell()
        {
            var window = container.Resolve<DashBoard>();

            window.Show();

            return window;
        }

        private void ConfigureContainer()
        {
            RegisterCommons();
            RegisterDatabase();
            
            RegisterCQRSCore();
            
            RegisterViews();
            RegisterViewModels();
            RegisterModels(); // TODO: 
            RegisterControllers();
            
            RegisterDomain();
        }

        private void RegisterCommons()
        {
            TW.Commons.BootStrapper.Register(container);
        }

        private void RegisterDatabase()
        {
            // TODO: this combo works ok, need to flesh out the alternatives
            //TW.CQRS.Core.Data.Dapper.BootStrapper.RegisterForEventStorage(container);
            //TW.CQRS.Core.Data.NHib.BootStrapper.RegisterForReporting(container, new List<Assembly>() { typeof(Property).Assembly });

            // TODO: TBD
            // TW.CQRS.Core.Data.Sql.BootStrapper.RegisterForReporting(container);

            // Testing
            TW.CQRS.Core.Data.Memory.BootStrapper.RegisterForEventStorage(container);
            TW.CQRS.Core.Data.Memory.BootStrapper.RegisterForReporting(container);
        }

        private void RegisterCQRSCore()
        {
            TW.CQRS.Core.BootStrapper.Register(container);
        }

        private void RegisterViews()
        {
            container.RegisterType<DashBoard>();
        }

        private void RegisterViewModels()
        {
            container.RegisterType<DashBoardViewModel>();
        }

        private void RegisterModels()
        {
            // ..
        }

        private void RegisterControllers()
        {
            // .. 
        }

        private void RegisterDomain()
        {
            Domain.BootStrapper.Register(container);
        }
    }
}
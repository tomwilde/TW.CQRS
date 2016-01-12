using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using log4net;

namespace EstateAgent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly BootStrapper bootStrapper = new BootStrapper();
        private readonly ILog logger;

        public App()
        {
            logger = LogManager.GetLogger(this.GetType());

            bootStrapper.Run(); 

            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(e.Exception);
        }

        protected UIElement RootVisual { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {            
            this.RootVisual = bootStrapper.ShowMainShell();
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            this.RootVisual = null;
        }
    }
}

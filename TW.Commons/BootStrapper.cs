using Microsoft.Practices.Unity;
using TW.Commons.Config;
using TW.Commons.Interfaces;
using TW.Commons.IoC;
using TW.Commons.Logging;
using TW.Commons.Serialisation;

namespace TW.Commons
{
    public static class BootStrapper
    {
         public static void Register(IUnityContainer container)
         {
             container.RegisterType<ILoggerFactory, Log4netLoggerFactory>();
             container.RegisterType<ILogger, Log4netLogger>();
             container.RegisterType<ISerialiser, Serialiser>();

             container.RegisterType<IConnectionStringProvider, ConnectionStringProvider>();

             container.RegisterType<IFactoryTypeContainer, FactoryTypeContainer>();
         }
    }
}
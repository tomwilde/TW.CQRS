using System.Configuration;
using TW.Commons.Interfaces;

namespace TW.Commons.Config
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
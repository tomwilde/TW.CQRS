using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EstateAgent.DeNormalisers
{
    public interface INormaliser
    {
        void Normalize(dynamic data, Func<dynamic, dynamic> map);

    }

    /// <summary>
    /// Pushes updates from the Domain over to the Query DB (can be server process / in-process...)
    /// </summary>
    public class Server
    {
        static void Main(string[] args)
        {
               

        }

        public Server()
        {
            // setup
        }

        public void Run()
        {
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}

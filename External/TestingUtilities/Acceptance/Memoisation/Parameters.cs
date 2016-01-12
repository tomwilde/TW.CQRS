using System;
using System.Collections.Generic;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    [Serializable]
    public class Parameters : AbstractCapturedData
    {
        public Parameters()
        {
        }

        public List<string> Types { get; set; }
        public object[] Data { get; set; }
    }
}
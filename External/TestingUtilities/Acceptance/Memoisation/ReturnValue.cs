using System;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    [Serializable]
    public class ReturnValue : AbstractCapturedData
    {
        public object Data { get; set; }
        public string Type { get; set; }
    }
}
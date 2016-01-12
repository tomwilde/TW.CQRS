using System;

namespace TW.Commons.TestingUtilities.Acceptance.Memoisation
{
    [Serializable]
    public abstract class AbstractCapturedData
    {
        public int CallNumber { get; set; }
        public string Method { get; set; }
    }
} 
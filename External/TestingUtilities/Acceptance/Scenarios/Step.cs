using System;

namespace TW.Commons.TestingUtilities.Acceptance.Scenarios
{
    public class Step 
    {
        public StepName Type;
        public string Msg { get; set; }

        public Delegate StepCall { get; set; }
    }
}
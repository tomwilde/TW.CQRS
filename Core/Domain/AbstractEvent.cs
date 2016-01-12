using System;

namespace TW.CQRS.Core.Domain
{
    [Serializable]
    public abstract class AbstractEvent
    {
        protected AbstractEvent()
        {
            TimeStamp = DateTime.Now;
        }

        public int Id
        {
            get; set;
        }

        public int AggregateId
        {
            get; set;
        }

        public DateTime TimeStamp
        {
            get; set;
        }
    }
}
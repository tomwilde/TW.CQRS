using System;

namespace TW.CQRS.Core
{
    public class Event : ICloneable
    {
        public virtual int Id {get;set;}
        public virtual int AggregateId {get;set;}
        public virtual string EventType {get;set;}
        public virtual string EventData {get;set;}
        public virtual DateTime Timestamp {get;set;}

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}", Id, AggregateId, Timestamp, EventType, EventData);
        }

        public object Clone()
        {
            return this.MemberwiseClone() as Event;
        }
    }
}
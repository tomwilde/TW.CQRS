using System;

namespace TW.Commons.Events
{
    public class EventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        public EventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
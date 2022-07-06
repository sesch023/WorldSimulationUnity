using System.Collections.Generic;
using Manager;

namespace Base
{
    public class TimeEvent : TickEvent
    {
        public TimeEvent(long atTime, OnRemoval managedRemoval = null) : base(atTime, managedRemoval)
        {
        }

        public TimeEvent(long atTime, Subscriber subscriber, OnRemoval managedRemoval = null)
            : base(atTime, subscriber, managedRemoval)
        {
        }

        public TimeEvent(long atTime, IEnumerable<Subscriber> subscribers, OnRemoval managedRemoval = null)
            : base(atTime, subscribers, managedRemoval)
        {
        }

        protected override long GetTimeCondition()
        {
            return TimeManager.Instance.PassedTime;
        }
    }
}
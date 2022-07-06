using System.Collections.Generic;
using Manager;

namespace Base
{
    public class TimeSpacedEvent : TickSpacedEvent
    {
        public TimeSpacedEvent(long timeSpacing) : base(timeSpacing)
        {
        }

        public TimeSpacedEvent(long timeSpacing, Subscriber subscriber) : base(timeSpacing, subscriber)
        {
        }

        public TimeSpacedEvent(long timeSpacing, IEnumerable<Subscriber> subscribers) : base(timeSpacing, subscribers)
        {
        }

        protected override long GetTimeCondition(long lastTime)
        {
            return TimeManager.Instance.TimePassedSince(lastTime);
        }
    }
}
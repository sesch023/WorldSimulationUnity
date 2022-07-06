using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Base
{
    public class TickSpacedEvent : UpdatableEvent
    {
        protected long LastTick;
        protected long TickSpacing;

        public TickSpacedEvent(long tickSpacing)
        {
            Init(tickSpacing);
        }

        public TickSpacedEvent(long tickSpacing, Subscriber subscriber) : base(subscriber)
        {
            Init(tickSpacing);
        }

        public TickSpacedEvent(long tickSpacing, IEnumerable<Subscriber> subscribers) : base(subscribers)
        {
            Init(tickSpacing);
        }

        private void Init(long tickSpacing)
        {
            TickSpacing = tickSpacing;
            LastTick = TimeManager.Instance.PassedTicks;
        }

        protected virtual long GetTimeCondition(long lastTick)
        {
            return TimeManager.Instance.TicksPassedSince(lastTick);
        }

        private bool UpdateCondition()
        {
            var passed = GetTimeCondition(LastTick);
            var condition = passed >= TickSpacing;
            if (condition)
                LastTick += passed;
            return condition;
        }

        public override void Update()
        {
            if (UpdateCondition()) TriggerSubscribers();
        }
    }
}
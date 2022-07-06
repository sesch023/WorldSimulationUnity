using System.Collections.Generic;
using Manager;

namespace Base
{
    public class TickEvent : UpdatableEvent, IManagedRemoval
    {
        private long _atTick;
        private OnRemoval _managedRemoval;
        private bool _triggered;

        public TickEvent(long atTick, OnRemoval managedRemoval = null)
        {
            Init(atTick, managedRemoval);
        }

        public TickEvent(long atTick, Subscriber subscriber, OnRemoval managedRemoval = null) : base(subscriber)
        {
            Init(atTick, managedRemoval);
        }

        public TickEvent(long atTick, IEnumerable<Subscriber> subscribers, OnRemoval managedRemoval = null)
            : base(subscribers)
        {
            Init(atTick, managedRemoval);
        }

        public void ManageRemoval()
        {
            _managedRemoval(this);
        }

        private void Init(long atTick, OnRemoval managedRemoval)
        {
            if (managedRemoval == null) managedRemoval = i => { };

            _managedRemoval = managedRemoval;
            _atTick = atTick;
        }

        protected virtual long GetTimeCondition()
        {
            return TimeManager.Instance.PassedTicks;
        }

        public override void Update()
        {
            if (GetTimeCondition() >= _atTick && !_triggered)
            {
                _triggered = true;
                TriggerSubscribers();
                ManageRemoval();
            }
        }
    }
}
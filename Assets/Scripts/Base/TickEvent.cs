using System.Collections.Generic;
using Manager;

namespace Base
{
    /// <summary>
    /// TickEvent which is triggered at a certain tick number once. Needs to be removed after it has been triggered,
    /// by giving a proper OnRemoval delegate. UpdateManager deals with triggering and
    /// will automatically call managed removal of the TickEvent. Automatically registers
    /// itself with the UpdateManager.
    /// </summary>
    public class TickEvent : UpdatableEvent, IManagedRemoval
    {
        /// Executed at this tick once.
        private long _atTick;
        /// Delegate to be executed on removal by the UpdateManager.
        private OnRemoval _managedRemoval;
        /// Was the event triggered already?
        private bool _triggered;
        
        /// <summary>
        /// Constructor of the TickEvent which is given the tick number it should be triggered at and a
        /// delegate to be executed on removal.
        /// </summary>
        /// <param name="atTick">Tick number the event should be triggered at.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager.</param>
        public TickEvent(long atTick, OnRemoval managedRemoval = null)
        {
            Init(atTick, managedRemoval);
        }

        /// <summary>
        /// Constructor of the TickEvent which is given the tick number it should be triggered at, a subscriber and a
        /// delegate to be executed on removal.
        /// </summary>
        /// <param name="atTick">Tick number the event should be triggered at.</param>
        /// <param name="subscriber">Subscriber to be added to the event.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager</param>
        public TickEvent(long atTick, Subscriber subscriber, OnRemoval managedRemoval = null) : base(subscriber)
        {
            Init(atTick, managedRemoval);
        }
        
        /// <summary>
        /// Constructor of the TickEvent which is given the tick number it should be triggered at, subscribers and a
        /// delegate to be executed on removal.
        /// </summary>
        /// <param name="atTick">Tick number the event should be triggered at.</param>
        /// <param name="subscribers">Subscribers to be added to the event.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager</param>
        public TickEvent(long atTick, IEnumerable<Subscriber> subscribers, OnRemoval managedRemoval = null)
            : base(subscribers)
        {
            Init(atTick, managedRemoval);
        }

        /// <summary>
        /// Triggeres the removal delegate.
        /// </summary>
        public void ManageRemoval()
        {
            _managedRemoval(this);
        }
        
        /// <summary>
        /// Inits the TickEvent. If the managed removal delegate is null it automatically triggers the removal by the
        /// update manager.
        /// </summary>
        /// <param name="atTick">Tick at which the event should be triggered.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager</param>
        private void Init(long atTick, OnRemoval managedRemoval)
        {
            if (managedRemoval == null) 
                managedRemoval = i => { UpdateManager.Instance.MarkRemovableForRemoval(i); };

            _managedRemoval = managedRemoval;
            _atTick = atTick;
        }
        
        /// <summary>
        /// Time Condition for the TickEvent at which the event should be triggered.
        /// </summary>
        /// <returns>Current Value of the Passed Ticks in the Time Manager.</returns>
        protected virtual long GetTimeCondition()
        {
            return TimeManager.Instance.PassedTicks;
        }

        /// <summary>
        /// Checks if tick condition is met and the event is not triggered yet. If so, it triggers the event and
        /// executes the removal delegate.
        /// </summary>
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
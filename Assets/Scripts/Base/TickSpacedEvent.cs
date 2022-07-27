using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// TickSpacedEvent which continously fires the event at a given interval. Automatically registers
    /// itself with the UpdateManager.
    /// </summary>
    public class TickSpacedEvent : UpdatableEvent
    {
        /// Last tick the event was fired.
        protected long LastTick;
        /// Spacings between ticks.
        protected long TickSpacing;

        /// <summary>
        /// Constructor of a TickSpacedEvent which continously fires the event at a given interval.
        /// </summary>
        /// <param name="tickSpacing">Spacing between fired ticks.</param>
        public TickSpacedEvent(long tickSpacing)
        {
            Init(tickSpacing);
        }

        /// <summary>
        /// Constructor of a TickSpacedEvent which continously fires the event at a given interval. Also
        /// adds a subscriber to the event.
        /// </summary>
        /// <param name="tickSpacing">Spacing between fired ticks.</param>
        /// <param name="subscriber">A Subscriber to the event.</param>
        public TickSpacedEvent(long tickSpacing, Subscriber subscriber) : base(subscriber)
        {
            Init(tickSpacing);
        }
        
        /// <summary>
        /// Constructor of a TickSpacedEvent which continously fires the event at a given interval. Also
        /// adds subscribers to the event.
        /// </summary>
        /// <param name="tickSpacing"></param>
        /// <param name="subscribers"></param>
        public TickSpacedEvent(long tickSpacing, IEnumerable<Subscriber> subscribers) : base(subscribers)
        {
            Init(tickSpacing);
        }
        
        /// <summary>
        /// Inits the TickSpacedEvent.
        /// </summary>
        /// <param name="tickSpacing">Spacing between fired ticks.</param>
        private void Init(long tickSpacing)
        {
            TickSpacing = tickSpacing;
            LastTick = TimeManager.Instance.PassedTicks;
        }
        
        /// <summary>
        /// Conditional number for the firing check. Asks the TimeManager for the passed ticks.
        /// </summary>
        /// <param name="lastTick">Last tick the event was fired.</param>
        /// <returns>Passed Ticks since the last firing.</returns>
        protected virtual long GetTimeCondition(long lastTick)
        {
            return TimeManager.Instance.TicksPassedSince(lastTick);
        }
        
        /// <summary>
        /// Update Condition for the TickSpacedEvent.
        /// </summary>
        /// <returns>True if the Event should be fired.</returns>
        private bool UpdateCondition()
        {
            var passed = GetTimeCondition(LastTick);
            var condition = passed >= TickSpacing;
            if (condition)
                LastTick += passed;
            return condition;
        }
        
        /// <summary>
        /// Triggers the subscribes if the condition is met.
        /// </summary>
        public override void Update()
        {
            if (UpdateCondition()) TriggerSubscribers();
        }
    }
}
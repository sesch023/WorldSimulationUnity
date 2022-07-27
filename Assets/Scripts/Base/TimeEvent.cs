using System.Collections.Generic;
using Manager;

namespace Base
{
    /// <summary>
    /// Time Event which fires at a specific passed time in milliseconds. Needs to be removed after it has been triggered,
    /// by giving a proper OnRemoval delegate. UpdateManager deals with triggering and
    /// will automatically call managed removal of the TickEvent. Automatically registers
    /// itself with the UpdateManager.
    /// </summary>
    public class TimeEvent : TickEvent
    {
        /// <summary>
        /// Constructor of a time event which fires at a specific passed time in milliseconds. Given a time in ms and a
        /// delegate to be executed on removal.
        /// </summary>
        /// <param name="atTime">Passed time in ms the event is triggered at.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager.</param>
        public TimeEvent(long atTime, OnRemoval managedRemoval = null) : base(atTime, managedRemoval)
        {
        }

        /// <summary>
        /// Constructor of a time event which fires at a specific passed time in milliseconds. Given a time in ms and a
        /// delegate to be executed on removal. Also adds a single subscriber to the event.
        /// </summary>
        /// <param name="atTime">Passed time in ms the event is triggered at.</param>
        /// <param name="subscriber">Subscriber to be added.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager.</param>
        public TimeEvent(long atTime, Subscriber subscriber, OnRemoval managedRemoval = null)
            : base(atTime, subscriber, managedRemoval)
        {
        }
        
        /// <summary>
        /// Constructor of a time event which fires at a specific passed time in milliseconds. Given a time in ms and a
        /// delegate to be executed on removal. Also adds a single subscriber to the event.
        /// </summary>
        /// <param name="atTime">Passed time in ms the event is triggered at.</param>
        /// <param name="subscribers">Subscribers to be added.</param>
        /// <param name="managedRemoval">Delegate to be executed on removal by the UpdateManager. If null it automatically
        /// triggers the removal by the UpdateManager.</param>
        public TimeEvent(long atTime, IEnumerable<Subscriber> subscribers, OnRemoval managedRemoval = null)
            : base(atTime, subscribers, managedRemoval)
        {
        }
        
        /// <summary>
        /// Conditional number for the firing check. Asks the TimeManager for the passed time in ms.
        /// </summary>
        /// <returns>Passed Time.</returns>
        protected override long GetTimeCondition()
        {
            return TimeManager.Instance.PassedTime;
        }
    }
}
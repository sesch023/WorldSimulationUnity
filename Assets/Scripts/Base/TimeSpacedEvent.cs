using System.Collections.Generic;
using Manager;

namespace Base
{
    /// <summary>
    /// TimeSpacedEvent which is fired after a certain amount of time has passed continuously. Automatically registers
    /// itself with the UpdateManager.
    /// </summary>
    public class TimeSpacedEvent : TickSpacedEvent
    {
        /// <summary>
        /// Constructor of a TimeSpacedEvent which a given spacing in ms between each execution.
        /// </summary>
        /// <param name="timeSpacing">Time in ms between each execution.</param>
        public TimeSpacedEvent(long timeSpacing) : base(timeSpacing)
        {
        }

        /// <summary>
        /// Constructor of a TimeSpacedEvent which a given spacing in ms between each execution. Also given a
        /// single subscriber to the event.
        /// </summary>
        /// <param name="timeSpacing">Time in ms between each execution.</param>
        /// <param name="subscriber">Single subscriber to the event.</param>
        public TimeSpacedEvent(long timeSpacing, Subscriber subscriber) : base(timeSpacing, subscriber)
        {
        }
        
        /// <summary>
        /// Constructor of a TimeSpacedEvent which a given spacing in ms between each execution. Also given
        /// subscribers to the event.
        /// </summary>
        /// <param name="timeSpacing">Time in ms between each execution.</param>
        /// <param name="subscribers">Subscribers to the event.</param>
        public TimeSpacedEvent(long timeSpacing, IEnumerable<Subscriber> subscribers) : base(timeSpacing, subscribers)
        {
        }
        
        /// <summary>
        /// Return the time passed since the last execution.
        /// </summary>
        /// <param name="lastTime">Last time the event was executed.</param>
        /// <returns>Time passed since last execution.</returns>
        protected override long GetTimeCondition(long lastTime)
        {
            return TimeManager.Instance.TimePassedSince(lastTime);
        }
    }
}
using System.Collections.Generic;
using Manager;

namespace Base
{
    /// <summary>
    /// Abstract Updatable Event which can be triggered on certain conditions in the update method.
    /// </summary>
    public abstract class UpdatableEvent : Event, IUpdatable
    {
        /// <summary>
        /// Constructor for UpdatableEvent.
        /// </summary>
        public UpdatableEvent()
        {
            Init();
        }
        
        /// <summary>
        /// Constructor for UpdatableEvent initializing the base event with a single subscriber.
        /// </summary>
        /// <param name="subscriber">One subscriber to the event.</param>
        public UpdatableEvent(Subscriber subscriber) : base(subscriber)
        {
            Init();
        }
        
        /// <summary>
        /// Constructor for UpdatableEvent initializing the base event with multiple subscribers.
        /// </summary>
        /// <param name="subscribers">IEnumerable of subscribers to the event.</param>
        public UpdatableEvent(IEnumerable<Subscriber> subscribers) : base(subscribers)
        {
            Init();
        }
        
        /// <summary>
        /// Initializes the event by registering it with the UpdateManager.
        /// </summary>
        private void Init()
        {
            UpdateManager.Instance.RegisterUpdatable(this);
        }
        
        /// <summary>
        /// Method to be called by the UpdateManager every single update.
        /// </summary>
        public abstract void Update();
    }
}
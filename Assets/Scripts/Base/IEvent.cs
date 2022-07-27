using System.Collections.Generic;

namespace Base
{
    /// <summary>
    /// Base Interface for all Events.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Triggers the subscribers of the event, with a method call.
        /// </summary>
        public void TriggerSubscribers();
        
        /// <summary>
        /// Adds a single subscriber to the subscribers.
        /// </summary>
        /// <param name="subscriber">Subscriber to add.</param>
        public void AddSubscriber(Subscriber subscriber);
        
        /// <summary>
        /// Adds a IEnumerable of subscribers to the subscribers.
        /// </summary>
        /// <param name="subscribers">IEnumerable of subscribers to add.</param>
        public void AddSubscribers(IEnumerable<Subscriber> subscribers);
        
        /// <summary>
        /// Removes a subscriber from the subscribers by address.
        /// </summary>
        /// <param name="subscriber">Subscriber to remove.</param>
        public void RemoveSubscriber(Subscriber subscriber);
    }
}
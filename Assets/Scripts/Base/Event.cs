using System.Collections.Generic;

namespace Base
{
    /// <summary>
    /// Subscriber Delegate for an event.
    /// </summary>
    public delegate void Subscriber(Event triggeredBy);
    
    /// <summary>
    /// Event, which triggers subscribers, which can be registered and unregistered. Implements IEvent.
    /// </summary>
    public class Event : IEvent
    {
        /// List of subscribers.
        private readonly List<Subscriber> _subscribers;
        
        /// <summary>
        /// Constructor of the Event. Initializes the list of subscribers.
        /// </summary>
        public Event()
        {
            _subscribers = new List<Subscriber>();
        }
        
        /// <summary>
        /// Constructor of the Event. Initializes the list of subscribers with a single given subscriber.
        /// </summary>
        /// <param name="subscriber">Single Subscriber to add on construction.</param>
        public Event(Subscriber subscriber) : this()
        {
            AddSubscriber(subscriber);
        }
        
        /// <summary>
        /// Constructor of the Event. Initializes the list of subscribers with a IEnumerable of given subscribers.
        /// </summary>
        /// <param name="subscribers">List of subscribers to add on construction.</param>
        public Event(IEnumerable<Subscriber> subscribers) : this()
        {
            AddSubscribers(subscribers);
        }
        
        /// <summary>
        /// Triggers the subscribers of the event, with a method call.
        /// </summary>
        public void TriggerSubscribers()
        {
            foreach (var subscriber in _subscribers) subscriber(this);
        }
        
        /// <summary>
        /// Adds a single subscriber to the list of subscribers.
        /// </summary>
        /// <param name="subscriber">Subscriber to add.</param>
        public void AddSubscriber(Subscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }
        
        /// <summary>
        /// Adds a IEnumerable of subscribers to the list of subscribers.
        /// </summary>
        /// <param name="subscribers">IEnumerable of subscribers to add.</param>
        public void AddSubscribers(IEnumerable<Subscriber> subscribers)
        {
            _subscribers.AddRange(subscribers);
        }
        
        /// <summary>
        /// Removes a subscriber from the list of subscribers by address.
        /// </summary>
        /// <param name="subscriber">Subscriber to remove.</param>
        public void RemoveSubscriber(Subscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }
    }
}
using System.Collections.Generic;

namespace Base
{
    public delegate void Subscriber(Event triggeredBy);

    public abstract class Event : IEvent
    {
        private readonly List<Subscriber> _subscribers;

        public Event()
        {
            _subscribers = new List<Subscriber>();
        }

        public Event(Subscriber subscriber) : this()
        {
            AddSubscriber(subscriber);
        }

        public Event(IEnumerable<Subscriber> subscribers) : this()
        {
            AddSubscribers(subscribers);
        }

        public void TriggerSubscribers()
        {
            foreach (var subscriber in _subscribers) subscriber(this);
        }

        public void AddSubscriber(Subscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void AddSubscribers(IEnumerable<Subscriber> subscribers)
        {
            _subscribers.AddRange(subscribers);
        }

        public void RemoveSubscriber(Subscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }
    }
}
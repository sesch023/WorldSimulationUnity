using System.Collections.Generic;

namespace Base
{
    public interface IEvent
    {
        public void TriggerSubscribers();

        public void AddSubscriber(Subscriber subscriber);

        public void AddSubscribers(IEnumerable<Subscriber> subscribers);

        public void RemoveSubscriber(Subscriber subscriber);
    }
}
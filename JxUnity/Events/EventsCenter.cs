
using System.Collections.Generic;

namespace JxUnity.Events
{
    public delegate void Event(EventArgsBase args);

    public class EventTrigger
    {
        public string Id { get; set; }
        public event Event Receiver;
        public void Invoke(EventArgsBase e = null)
        {
            this.Receiver?.Invoke(e);
        }
    }

    public static class EventCenter
    {
        private static Dictionary<string, EventTrigger> events 
            = new Dictionary<string, EventTrigger>();

        public static void RegReceiver(string id, Event receiver)
        {
            EventTrigger trigger;
            if (!events.TryGetValue(id, out trigger))
            {
                trigger = new EventTrigger() { Id = id };
                events.Add(id, trigger);
            }

            trigger.Receiver += receiver;
        }
        public static void UnregReceiver(string id, Event receiver)
        {
            EventTrigger trigger;
            if (!events.TryGetValue(id, out trigger))
            {
                UnityEngine.Debug.LogWarning($"EventCenter id: {id} not found.");
                return;
            }
            trigger.Receiver -= receiver;
        }
        
        public static void Invoke(string id, EventArgsBase e = null)
        {
            EventTrigger trigger;
            if (events.TryGetValue(id, out trigger))
            {
                trigger.Invoke(e);
            }
        }

    }

}


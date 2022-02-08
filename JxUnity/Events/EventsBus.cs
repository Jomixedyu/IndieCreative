
using System;
using System.Collections.Generic;

namespace JxUnity.Events
{
    public delegate void Event(object sender, EventArgsBase args);

    public class EventArgsBase : System.EventArgs
    {
        public string EventId { get; set; }
    }
    public class CommonEventArgsBase : EventArgsBase
    {
        public object[] Args { get; set; }
    }

    internal sealed class EventTrigger
    {
        public string Id { get; set; }
        private List<Event> Receiver = new List<Event>();

        public void AddRecevier(Event e)
        {
            this.Receiver.Add(e);
        }
        public void RemoveReceiver(Event e)
        {
            this.Receiver.Remove(e);
            
        }

        public void Broadcast(object sender, EventArgsBase e = null)
        {
            foreach (Event receiver in this.Receiver)
            {
                receiver.Invoke(sender, e);
            }
        }
    }

    public static class EventBus
    {
        private static Dictionary<string, EventTrigger> events
            = new Dictionary<string, EventTrigger>();
        private static List<Event> listener = new List<Event>();

        public static void Subscribe(string id, Event receiver)
        {
            EventTrigger trigger;
            if (!events.TryGetValue(id, out trigger))
            {
                trigger = new EventTrigger() { Id = id };
                events.Add(id, trigger);
            }
            trigger.AddRecevier(receiver);
        }
        public static void Unsubscribe(string id, Event receiver)
        {
            EventTrigger trigger;
            if (!events.TryGetValue(id, out trigger))
            {
                UnityEngine.Debug.LogWarning($"[EventBus] id: {id} not found.");
                return;
            }
            trigger.RemoveReceiver(receiver);
        }

        public static void Broadcast(string id, object sender, EventArgsBase e = null)
        {
            EventTrigger trigger;
            if (events.TryGetValue(id, out trigger))
            {
                trigger.Broadcast(e);
            }
            foreach (var item in listener)
            {
                item.Invoke(sender, e);
            }
        }

        public static void AddHook(Event receiver)
        {
            listener.Add(receiver);
        }
        public static void RemoveHook(Event receiver)
        {
            listener.Remove(receiver);
        }
    }

}


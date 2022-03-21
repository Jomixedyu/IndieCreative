
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public delegate void EventHandler(string eventId, object sender, EventArgsBase args);

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
public sealed class EventHandlerAttribute : System.Attribute
{
    public string EventId { get; set; }
    public EventHandlerAttribute()
    {

    }

    public EventHandlerAttribute(string eventId)
    {
        this.EventId = eventId;
    }
}


public class EventHub
{
    protected sealed class EventTrigger
    {
        public string EventId { get; set; }
        private List<EventHandler> Receiver = new List<EventHandler>();

        public void AddRecevier(EventHandler e)
        {
            this.Receiver.Add(e);
        }

        public void RemoveReceiver(EventHandler e)
        {
            this.Receiver.Remove(e);

        }
        public void Broadcast(object sender, EventArgsBase e)
        {
            foreach (var receiver in this.Receiver)
            {
                receiver?.Invoke(this.EventId, sender, e);
            }
        }
    }


    private Dictionary<string, EventTrigger> events
        = new Dictionary<string, EventTrigger>();

    public void Subscribe(string eventId, EventHandler receiver)
    {
        EventTrigger trigger;
        if (!events.TryGetValue(eventId, out trigger))
        {
            trigger = new EventTrigger() { EventId = eventId };
            events.Add(eventId, trigger);
        }
        trigger.AddRecevier(receiver);
    }


    public void Unsubscribe(string eventId, EventHandler receiver)
    {
        EventTrigger trigger;
        if (!events.TryGetValue(eventId, out trigger))
        {
            UnityEngine.Debug.LogWarning($"[EventHub] id: {eventId} not found.");
            return;
        }
        trigger.RemoveReceiver(receiver);
    }

    public void Broadcast(string eventId, object sender, EventArgsBase arg)
    {
        EventTrigger trigger;
        if (events.TryGetValue(eventId, out trigger))
        {
            trigger.Broadcast(sender, arg);
        }
    }

    public void Subscribes(object obj)
    {
        System.Type type = obj.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            if (method.IsDefined(typeof(EventHandlerAttribute), true))
            {
                var e = method.GetCustomAttribute<EventHandlerAttribute>(true);
                var deleg = method.CreateDelegate(typeof(EventHandler), obj) as EventHandler;
                Subscribe(e.EventId, deleg);
            }
        }
    }
    public void Unsubscribes(object obj)
    {
        System.Type type = obj.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            if (method.IsDefined(typeof(EventHandlerAttribute), true))
            {
                var e = method.GetCustomAttribute<EventHandlerAttribute>(true);
                var deleg = method.CreateDelegate(typeof(EventHandler), obj) as EventHandler;
                Unsubscribe(e.EventId, deleg);
            }
        }
    }

}

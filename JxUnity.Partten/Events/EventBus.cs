using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{

    private static List<EventHub> hubs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Init()
    {
        hubs = new List<EventHub>();
    }


    public static void AddEventHub(EventHub hub)
    {
        hubs.Add(hub);
    }
    public static void RemoveEventHub(EventHub hub)
    {
        hubs.Remove(hub);
    }

    public static void Broadcast(string eventId, object sender, EventArgsBase e)
    {
        foreach (var hub in hubs)
        {
            hub.Broadcast(eventId, sender, e);
        }
    }
}
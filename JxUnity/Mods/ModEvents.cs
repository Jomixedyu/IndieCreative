using System;
using System.Collections.Generic;

namespace JxUnity.Mods
{
    public class ModEventArgs : EventArgsBase
    {
    }

    public class ModEventBus : EventHub
    {
        private static ModEventBus instance;
        public static ModEventBus Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModEventBus(); 
                }
                return instance;
            }
        }

    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModEventHandlerAttribute : Attribute
    {
        public string EventId { get; set; }
        public ModEventHandlerAttribute()
        {

        }

        public ModEventHandlerAttribute(string eventId)
        {
            this.EventId = eventId;
        }
    }

}

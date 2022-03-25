using System;


namespace JxUnity.Mods
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModEventHandlerAttribute : EventHandlerAttribute
    {
        public ModEventHandlerAttribute(string eventId) : base(eventId)
        {
        }
    }
}

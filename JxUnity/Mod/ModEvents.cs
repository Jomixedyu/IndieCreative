using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JxUnity.Exentsions
{
    public class ModEventArgs : IDisposable
    {
        public virtual void Dispose()
        {
        }
    }

    public delegate void ModEventHandler(object sender, ModEventArgs _e);

    //鉴于Mod功能叠加与修改的复杂性，Mod应有严谨的启动顺序，应不支持在游戏过程中对Mod的加载与卸载，仅在启动时对Mod进行顺序加载。
    public class ModInfo
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Version { get; protected set; }
        public string Author { get; private set; }
        public int Order { get; set; }
    }

    internal class ModEventTrigger
    {
        private List<ModEventHandler> Receiver = new List<ModEventHandler>();
        public void Add(ModEventHandler receiver)
        {
            this.Receiver.Add(receiver);
        }
        public void Remove(ModEventHandler receiver)
        {
            this.Receiver.Remove(receiver);
        }
        public void Broadcast(object sender, ModEventArgs e)
        {
            foreach (var receiver in this.Receiver)
            {
                receiver.Invoke(sender, e);
            }
        }
    }

    public class ModEventBus
    {
        private static Dictionary<string, ModEventTrigger> Channels
            = new Dictionary<string, ModEventTrigger>();
        public static void Subscribe(string eventId, ModEventHandler receiver)
        {
            ModEventTrigger trigger;
            if (!Channels.TryGetValue(eventId, out trigger))
            {
                trigger = new ModEventTrigger();
                Channels.Add(eventId, trigger);
            }
            trigger.Add(receiver);
        }
        public static void Unsubscribe(string eventId, ModEventHandler receiver)
        {
            ModEventTrigger trigger;
            if (!Channels.TryGetValue(eventId, out trigger))
            {
                return;
            }
            trigger.Remove(receiver);
        }

        public static void Broadcast(string eventId, object sender, ModEventArgs e)
        {
            if (Channels.TryGetValue(eventId, out var trigger))
            {
                trigger.Broadcast(sender, e);
            }
        }
    }

    public static class ModLoader
    {
        public static void LoadMod(Type type)
        {
            var inst = (ModBase)Activator.CreateInstance(type);

        }
        public static void Load(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsDefined(typeof(ModAttribute), false))
                {
                    LoadMod(item);
                }
            }
        }
    }

    public abstract class ModBase
    {
        public ModInfo Info { get; protected set; }
        public ModBase()
        {
            foreach (var item in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (!item.IsDefined(typeof(ModEventHandlerAttribute)))
                {
                    continue;
                }
                var attr = item.GetCustomAttribute<ModEventHandlerAttribute>();
                string eventId = attr.EventId ?? item.Name;
                var delg = (ModEventHandler)item.CreateDelegate(typeof(ModEventHandler), this);
                ModEventBus.Subscribe(eventId, delg);
            }
        }

    }



    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModAttribute : Attribute
    {

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


    public class OnGameSaveArgs : ModEventArgs
    {
        public bool isBreak;
    }

    [Mod]
    public class ExampleMod : ModBase
    {

        [ModEventHandler]
        private void OnGameSaved(object sender, ModEventArgs args)
        {
            var save = args as OnGameSaveArgs;
            ModEventBus.Broadcast("asd", null, null);
        }
    }

}

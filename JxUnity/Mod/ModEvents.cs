using System;
using System.Collections.Generic;
using System.Linq;

namespace JxUnity.Exentsions
{
    public class ModEventArgs
    {
    }

    public delegate void ModEvent(object sender, ModEventArgs _e);

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
        private List<ModEvent> Receiver = new List<ModEvent>();
        public void Add(ModEvent receiver)
        {
            this.Receiver.Add(receiver);
        }
        public void Remove(ModEvent receiver)
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

        public static void Subscribe(string eventId, ModEvent receiver)
        {
            ModEventTrigger trigger;
            if (!Channels.TryGetValue(eventId, out trigger))
            {
                trigger = new ModEventTrigger();
                Channels.Add(eventId, trigger);
            }
            trigger.Add(receiver);
        }
        public static void Unsubscribe(string eventId, ModEvent receiver)
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

    public abstract class ModBase
    {
        public ModInfo Info { get; protected set; }
        public virtual void OnIntialize() { }
        public virtual void OnTerminate() { }
    }

    public class OnGameSaveArgs : ModEventArgs
    {
        public bool isBreak;
    }

    public class ExampleExtension : ModBase
    {
        public override void OnIntialize()
        {
            ModEventBus.Subscribe("on_game_save", OnTake);
        }

        private void OnTake(object sender, ModEventArgs args)
        {
            var save = args as OnGameSaveArgs;
            ModEventBus.Broadcast("asd", null, null);
        }
    }

}

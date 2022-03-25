using System;
using System.Collections.Generic;

namespace JxUnity.Pools
{
    public class ListPool
    {
        public struct ListPoolItemRAII<T> : IDisposable
        {
            private readonly List<T> item;
            public List<T> Item { get => item; }
            public ListPoolItemRAII(List<T> item)
            {
                this.item = item;
            }

            public void Dispose()
            {
                ListPool.Recycle(this.item);
            }
        }

        protected static Dictionary<Type, List<object>> pools = new Dictionary<Type, List<object>>();

        public static void ReleaseAll()
        {
            pools = new Dictionary<Type, List<object>>();
        }
        public static ListPoolItemRAII<T> Use<T>()
        {
            return new ListPoolItemRAII<T>(Get<T>());
        }

        public static List<T> Get<T>()
        {
            Type type = typeof(T);
            if (pools.TryGetValue(type, out var v))
            {
                if (v.Count == 0)
                {
                    return new List<T>();
                }
                int last = v.Count - 1;
                var r = (List<T>)v[last];
                v.RemoveAt(last);
                return r;
            }
            return new List<T>();
        }

        public static void Recycle<T>(List<T> obj)
        {
            Type type = obj.GetType();
            obj.Clear();
            if (!pools.ContainsKey(type))
            {
                pools.Add(type, new List<object>());
            }
            pools[type].Add(obj);
        }
    }
}
using System;
using System.Collections.Generic;

namespace JxUnity.SaveDatas
{
    internal abstract class Value
    {
        public virtual string TypeName { get; }
        public virtual object ObjValue { get; }
    }

    internal class Value<T> : Value
    {
        public T value;
        public override string TypeName => value.GetType().Name;
        public override object ObjValue => value;

        public Value<T> SetValue(T value)
        {
            this.value = value;
            return this;
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    internal sealed class ValuesPool
    {
        private static Dictionary<Type, Queue<Value>> pools
            = new Dictionary<Type, Queue<Value>>();

        public static T Pop<T>() where T : Value, new()
        {
            if (pools.TryGetValue(typeof(T), out Queue<Value> pool))
            {
                return pool.Dequeue() as T;
            }
            return new T();
        }
        public static void Push(Value obj)
        {
            if (!pools.TryGetValue(obj.GetType(), out Queue<Value> pool))
            {
                pool = new Queue<Value>();
                pools.Add(obj.GetType(), pool);
            }
            pool.Enqueue(obj);
        }

        private static Value _PopOrCreate<T>(T value)
        {
            if (pools.TryGetValue(typeof(Value<T>), out Queue<Value> pool))
            {
                var v = pool.Count > 0 ? (Value<T>)pool.Dequeue() : new Value<T>();
                return v.SetValue(value);
            }
            return new Value<T>().SetValue(value);
        }
        public static Value PopOrCreate(string type, string value)
        {
            switch (type)
            {
                case "String": return _PopOrCreate(value);
                case "Int32": return _PopOrCreate(int.Parse(value));
                case "Single": return _PopOrCreate(float.Parse(value));
                case "Boolean": return _PopOrCreate(bool.Parse(value));
            }
            return null;
        }
        public static void Clear()
        {
            pools.Clear();
        }
    }

}

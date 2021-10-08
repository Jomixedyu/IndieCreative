using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 简易对象的对象池，回收时执行构造函数设置默认值（释放引用）。
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ObjectPool<T> where T : class, new()
{
    private static Queue<T> queue;
    private static ConstructorInfo ctorInfo;
    public static int Count => queue.Count;

    static ObjectPool()
    {
        queue = new Queue<T>();
        ctorInfo = typeof(T).GetConstructor(new Type[0]);
    }

    public static T Get()
    {
        if (queue.Count == 0)
        {
            return new T();
        }
        return queue.Dequeue();
    }

    public static void Recycle(T obj)
    {
        ctorInfo.Invoke(obj, null);
        queue.Enqueue(obj);
    }
    public static void RecycleRange(IEnumerable<T> enumerator)
    {
        var it = enumerator.GetEnumerator();
        while (it.MoveNext())
        {
            Recycle(it.Current);
        }
    }

}

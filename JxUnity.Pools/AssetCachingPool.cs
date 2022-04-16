using System;
using System.Collections.Generic;

namespace JxUnity.Pools
{
    public class AssetCachingPool
    {
        private static Dictionary<string, WeakReference<UnityEngine.Object>> objects
            = new Dictionary<string, WeakReference<UnityEngine.Object>>();

        public static bool TryGet(string name, out UnityEngine.Object obj)
        {
            if (objects.TryGetValue(name, out var v))
            {
                if (v.TryGetTarget(out var target))
                {
                    obj = target;
                    return true;
                }
                else
                {
                    //released
                    objects.Remove(name);
                }
            }
            obj = null;
            return false;
        }
        public static bool TryGet<T>(string name, out T obj) where T : UnityEngine.Object
        {
            UnityEngine.Object _obj;
            bool b = TryGet(name, out _obj);
            obj = (T)_obj;
            return b;
        }
    }
}

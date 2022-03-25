using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Pools
{
    internal sealed class GameObjectPoolMono : MonoBehaviour
    {
        private static GameObjectPoolMono instance;
        public static bool HasInstance => instance != null;

        public static GameObjectPoolMono Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_{nameof(GameObjectPoolMono)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<GameObjectPoolMono>();
                }
                return instance;
            }
        }

        private Dictionary<string, GameObjectPoolItem> pools;

        private void Awake()
        {
            this.pools = new Dictionary<string, GameObjectPoolItem>();
        }
        /// <summary>
        /// 按对象获取池类型，如果不存在，则返回null
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public string GetObjectType(GameObject go)
        {
            foreach (var item in this.pools)
            {
                if (item.Value.IsInPool(go.GetInstanceID()))
                {
                    return item.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 某类型池是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsExist(string type)
        {
            return this.pools.ContainsKey(type);
        }

        /// <summary>
        /// 注册池对象
        /// </summary>
        /// <param name="type">池类型</param>
        /// <param name="go">对象原型</param>
        /// <param name="getCb">获取回调</param>
        /// <param name="recycleCb">回收回调</param>
        /// <returns></returns>
        public GameObjectPoolItem Register(
            string type,
            GameObject go,
            Action<GameObject> getCb,
            Action<GameObject> recycleCb)
        {
            GameObjectPoolItem pool = new GameObjectPoolItem(this.gameObject, type, go, getCb, recycleCb);
            this.pools.Add(type, pool);
            return pool;
        }

        /// <summary>
        /// 获取一个池内对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject Get(string type)
        {
            return this.pools[type].Get();
        }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="go"></param>
        public void Recycle(GameObject go)
        {
            string type = this.GetObjectType(go);
            if (type == null)
            {
                throw new ArgumentException("not exists.");
            }
            this.pools[type].Recycle(go);
        }
        /// <summary>
        /// 卸载未在使用的池对象
        /// </summary>
        /// <param name="type"></param>
        public void UnloadUnused(string type)
        {
            this.pools[type].UnloadUnused();
        }
        /// <summary>
        /// 强制卸载池内对象
        /// </summary>
        /// <param name="type"></param>
        public void ForceUnload(string type)
        {
            this.pools[type].ForceUnload();
        }
        /// <summary>
        /// 强制卸载所有池内对象
        /// </summary>
        public void ForceUnloadAll()
        {
            foreach (var item in this.pools)
            {
                item.Value.ForceUnload();
            }
        }
        /// <summary>
        /// 删除池
        /// </summary>
        /// <param name="type"></param>
        public void Delete(string type)
        {
            if (!this.pools.ContainsKey(type))
            {
                return;
            }
            this.pools[type].Dispose();
            this.pools.Remove(type);
        }

        /// <summary>
        /// 删除所有池
        /// </summary>
        public void DeleteAll()
        {
            foreach (var item in this.pools)
            {
                item.Value.Dispose();
            }
            this.pools.Clear();
        }

        public int GetCount(string type)
        {
            return this.pools[type].ObjectCount;
        }

        internal GameObjectPoolItem GetPool(string type)
        {
            return this.pools[type];
        }

        public void ForEach(Action<GameObjectPoolItem> act)
        {
            foreach (var item in this.pools)
            {
                act?.Invoke(item.Value);
            }
        }

        public int GetUsableCount(string type)
        {
            return this.pools[type].UsableCount;
        }
    }
}
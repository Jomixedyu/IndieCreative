using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JxUnity.Pool
{
    public class GameObjectPoolItem
    {
        private class PoolItem
        {
            public bool isUsed;
            public GameObject gameObject;

            public PoolItem(bool isUsed, GameObject gameObject)
            {
                this.isUsed = isUsed;
                this.gameObject = gameObject;
            }
        }

        private GameObject parent;
        private string type;
        private GameObject proto;
        private GameObject _gameObject;
        private Action<GameObject> getCb;
        private Action<GameObject> recycleCb;

        public GameObject Parent { get => this.parent; }
        public string Type { get => this.type; }
        public GameObject Proto { get => this.proto; }
        public GameObject gameObject { get => _gameObject; }

        private Dictionary<int, PoolItem> pool = new Dictionary<int, PoolItem>(256);

        internal GameObjectPoolItem(
            GameObject parent,
            string type,
            GameObject proto,
            Action<GameObject> getCb,
            Action<GameObject> recycleCb)
        {
            this._gameObject = new GameObject(type);
            this._gameObject.transform.SetParent(parent.transform);

            this.parent = parent;
            this.type = type;
            this.proto = proto;
            this.getCb = getCb;
            this.recycleCb = recycleCb;
        }

        public int ObjectCount
        {
            get
            {
                return pool.Count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInPool(int instanceId)
        {
            return this.pool.ContainsKey(instanceId);
        }
        /// <summary>
        /// 预分配对象
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="cb">每生成一个对象时的回调</param>
        public void PreLoad(int count, Action<GameObject> cb = null)
        {
            for (int i = 0; i < count; i++)
            {
                this.Alloc(cb);
            }
        }

        /// <summary>
        /// unload unused objects
        /// </summary>
        /// <returns></returns>
        public int UnloadUnused()
        {
            List<int> unusedList = new List<int>();
            foreach (var item in this.pool)
            {
                if (item.Value.isUsed)
                {
                    unusedList.Add(item.Key);
                }
            }

            foreach (var item in unusedList)
            {
                this.pool.Remove(item);
            }

            return unusedList.Count;
        }

        public void ForceUnload()
        {
            foreach (var item in this.pool)
            {
                UnityEngine.Object.Destroy(item.Value.gameObject);
            }
            this.pool.Clear();
        }

        private int FindUnused()
        {
            foreach (var item in this.pool)
            {
                if (!item.Value.isUsed)
                {
                    return item.Key;
                }
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Alloc(Action<GameObject> cb = null)
        {
            GameObject go = UnityEngine.Object.Instantiate(this.proto, this._gameObject.transform);
            this.recycleCb?.Invoke(go);
            int id = go.GetInstanceID();
            this.pool.Add(id, new PoolItem(false, go));
            cb?.Invoke(go);
            return id;
        }

        public GameObject Get()
        {
            int id = this.FindUnused();
            if (id == 0)
            {
                id = this.Alloc();
            }
            var item = this.pool[id];
            item.isUsed = true;

            this.getCb?.Invoke(item.gameObject);
            return item.gameObject;
        }

        public void Recycle(GameObject go)
        {
            this.recycleCb?.Invoke(go);
            go.transform.SetParent(this._gameObject.transform);
            this.pool[go.GetInstanceID()].isUsed = false;
        }

        public int UsableCount
        {
            get
            {
                int i = 0;
                foreach (var item in this.pool)
                {
                    if (item.Value.isUsed)
                    {
                        ++i;
                    }
                }
                return i;
            }
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            this.pool = null;
            UnityEngine.Object.Destroy(this._gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameObjectPool : IDisposable
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

    public GameObjectPool(
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

    public int GetObjectCount()
    {
        return pool.Count;
    }

    public bool IsInPool(int instanceId)
    {
        return this.pool.ContainsKey(instanceId);
    }

    public void PreLoad(int count)
    {
        for (int i = 0; i < count; i++)
        {
            this.Alloc();
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

    public void ForceUnloadAll()
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

    private int Alloc()
    {
        GameObject go = UnityEngine.Object.Instantiate(this.proto, this._gameObject.transform);
        this.recycleCb?.Invoke(go);
        int id = go.GetInstanceID();
        this.pool.Add(id, new PoolItem(false, go));
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
        go.transform.SetParent(this._gameObject.transform);
        this.recycleCb?.Invoke(go);
        this.pool[go.GetInstanceID()].isUsed = false;
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

public sealed class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
{
    private Dictionary<string, GameObjectPool> pools;

    private void Awake()
    {
        this.pools = new Dictionary<string, GameObjectPool>();
    }

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

    public GameObjectPool Register(string type, GameObject go, Action<GameObject> getCb, Action<GameObject> recycleCb)
    {
        GameObjectPool pool = new GameObjectPool(this.gameObject, type, go, getCb, recycleCb);
        this.pools.Add(type, pool);
        return pool;
    }

    public GameObject Get(string type)
    {
        return this.pools[type].Get();
    }

    public void Recycle(GameObject go)
    {
        string type = this.GetObjectType(go);
        if (type == null)
        {
            throw new ArgumentException("not exists.");
        }
        this.pools[type].Recycle(go);
    }

    public void UnloadUnused(string type)
    {
        this.pools[type].UnloadUnused();
    }

    public void ForceUnloadAll(string type)
    {
        this.pools[type].ForceUnloadAll();
    }

    public void Delete(string type)
    {
        this.pools[type].Dispose();
        this.pools.Remove(type);
    }
}



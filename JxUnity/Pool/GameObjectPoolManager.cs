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

    protected override void Awake()
    {
        if (CheckInstanceAndDestroy())
        {
            return;
        }
        base.Awake();
        this.pools = new Dictionary<string, GameObjectPool>();
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

    private static void DefaultGetCallBack(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    private static void DefaultRecycleCallBack(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 注册池对象
    /// </summary>
    /// <param name="type">池类型</param>
    /// <param name="go">对象原型</param>
    /// <param name="getCb">获取回调</param>
    /// <param name="recycleCb">回收回调</param>
    /// <returns></returns>
    public GameObjectPool Register(
        string type,
        GameObject go,
        Action<GameObject> getCb,
        Action<GameObject> recycleCb)
    {
        GameObjectPool pool = new GameObjectPool(this.gameObject, type, go, getCb, recycleCb);
        this.pools.Add(type, pool);
        return pool;
    }
    /// <summary>
    /// 注册池对象，物体将自动激活与反激活。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObjectPool RegisterDefault(string type, GameObject go)
    {
        GameObjectPool pool = new GameObjectPool(this.gameObject, type, go, DefaultGetCallBack, DefaultRecycleCallBack);
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
}

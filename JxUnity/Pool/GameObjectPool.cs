using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public GameObjectPoolItem(
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

public static class GameObjectPool
{
    public static bool IsEnabled()
    {
        return GameObjectPoolMono.HasInstance;
    }
    /// <summary>
    /// 按对象获取池类型，如果不存在，则返回null
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetObjectType(GameObject go)
    {
        return GameObjectPoolMono.GetInstance().GetObjectType(go);
    }

    private static void DefaultGetCallBack(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    private static void DefaultRecycleCallBack(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private static GameObjectPoolItem Internal_Register(
        string type,
        GameObject go,
        Action<GameObject> getCb,
        Action<GameObject> recycleCb)
    {
        var inst = GameObjectPoolMono.GetInstance();
        if (inst.IsExist(type))
        {
            return null;
        }
        return inst.Register(type, go, getCb, recycleCb);
    }

    /// <summary>
    /// 注册池对象，如果对象池已存在，则返回null。
    /// </summary>
    /// <param name="type">池类型</param>
    /// <param name="go">对象原型</param>
    /// <param name="getCb">获取回调</param>
    /// <param name="recycleCb">回收回调</param>
    /// <returns></returns>
    public static GameObjectPoolItem Register(
        string type,
        GameObject go,
        Action<GameObject> getCb,
        Action<GameObject> recycleCb)
    {
        return Internal_Register(type, go, getCb, recycleCb);
    }
    /// <summary>
    /// 注册池对象，物体将自动激活与反激活。如果对象池已存在，则返回null。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObjectPoolItem RegisterDefault(string type, GameObject go)
    {
        return Internal_Register(type, go, DefaultGetCallBack, DefaultRecycleCallBack);
    }

    /// <summary>
    /// 获取一个池内对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static GameObject Get(string type)
    {
        return GameObjectPoolMono.GetInstance().Get(type);
    }
    public static int GetCount(string type)
    {
        return GameObjectPoolMono.GetInstance().GetCount(type);
    }
    public static GameObjectPoolItem GetPool(string type)
    {
        return GameObjectPoolMono.GetInstance().GetPool(type);
    }
    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="go"></param>
    public static void Recycle(GameObject go)
    {
        GameObjectPoolMono.GetInstance().Recycle(go);
    }

    /// <summary>
    /// 卸载未在使用的池对象
    /// </summary>
    /// <param name="type"></param>
    public static void UnloadUnused(string type)
    {
        GameObjectPoolMono.GetInstance().UnloadUnused(type);
    }
    /// <summary>
    /// 强制卸载池内对象
    /// </summary>
    /// <param name="type"></param>
    public static void ForceUnload(string type)
    {
        GameObjectPoolMono.GetInstance().ForceUnload(type);
    }
    /// <summary>
    /// 强制卸载所有池内对象
    /// </summary>
    public static void ForceUnloadAll()
    {
        GameObjectPoolMono.GetInstance().ForceUnloadAll();
    }
    /// <summary>
    /// 删除池
    /// </summary>
    /// <param name="type"></param>
    public static void Delete(string type)
    {
        GameObjectPoolMono.GetInstance().Delete(type);
    }

    /// <summary>
    /// 删除所有池
    /// </summary>
    public static void DeleteAll()
    {
        GameObjectPoolMono.GetInstance().DeleteAll();
    }
    /// <summary>
    /// 遍历池
    /// </summary>
    /// <param name="act"></param>
    public static void ForEach(Action<GameObjectPoolItem> act)
    {
        GameObjectPoolMono.GetInstance().ForEach(act);
    }
    /// <summary>
    /// 当前池内可用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetUsableCount(string type)
    {
        return GameObjectPoolMono.GetInstance().GetUsableCount(type);
    }
}

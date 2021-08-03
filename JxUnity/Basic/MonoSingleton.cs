using System;
using UnityEngine;

public abstract class MonoSingleton<T>
    : MonoBehaviour, IDisposable where T : MonoSingleton<T>
{
    [SerializeField]
    private bool IsDontDestroyOnInit = true;

    private static T mInstance = null;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (mInstance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    mInstance = go.AddComponent<T>();

                    GameObject parent = GameObject.Find("__System");
                    if (parent == null)
                    {
                        parent = new GameObject("__System");
                        DontDestroyOnLoad(parent);
                    }
                    go.transform.parent = parent.transform;
                }
                if (mInstance.IsDontDestroyOnInit)
                {
                    if (object.ReferenceEquals(mInstance.transform.parent, null))
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                }
            }
            return mInstance;
        }
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    protected virtual void Awake()
    {
        SetInstance(this as T);
        if (IsDontDestroyOnInit)
        {
            if (this.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else if (this.transform.parent.name == "__System")
            {
                DontDestroyOnLoad(this.transform.parent.gameObject);
            }
        }
    }

    public static T GetInstance()
    {
        return Instance;
    }
    protected void SetInstance(T obj)
    {
        mInstance = obj;
        isDisposed = false;
    }
    /// <summary>
    /// 检查单例实例是否已经存在，如果存在则销毁新的实例
    /// </summary>
    /// <returns>返回true为实例存在并已销毁</returns>
    protected bool CheckInstanceAndDestroy()
    {
        if (HasInstance)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (HasInstance)
        {
            if (mInstance == this)
            {
                mInstance = null;
            }
        }
    }

    protected bool isDisposed = false;
    public virtual void Dispose()
    {
        if (isDisposed)
        {
            return;
        }
        isDisposed = true;

        mInstance = null;
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}

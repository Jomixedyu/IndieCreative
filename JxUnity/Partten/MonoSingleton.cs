using System;
using UnityEngine;

public abstract class MonoSingleton<T>
    : MonoBehaviour where T : MonoSingleton<T>
{
    [SerializeField]
    private bool IsDontDestroyOnInit = true;
    [SerializeField]
    private bool IsKeepInstance = true;

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
                    GameObject go = new GameObject("__m_" + typeof(T).Name);
                    mInstance = go.AddComponent<T>();
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
        if (IsKeepInstance && KeepInstance())
        {
            //keep
            return;
        }
        else
        {
            MoveInstance();
        }

        if (IsDontDestroyOnInit)
        {
            if (this.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void OnMoveConstructor(T oldInstance)
    {

    }

    public static T GetInstance()
    {
        return Instance;
    }
    private void MoveInstance()
    {
        var old = mInstance;
        mInstance = this as T;
        OnMoveConstructor(old);
        if (old != null)
        {
            Destroy(old.gameObject);
        }
    }

    /// <summary>
    /// 保持实例，如果有新实例则会销毁，并返回true
    /// </summary>
    /// <returns></returns>
    private bool KeepInstance()
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

}

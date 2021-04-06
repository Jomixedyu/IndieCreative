using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T mInstance = null;

    public static T Instance
    {
        get => GetInstance();
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    public static T GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (mInstance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                mInstance = go.AddComponent<T>();
#if UNITY_EDITOR
                Debug.LogWarning("New Singleton Object " + typeof(T));
#endif
                GameObject parent = GameObject.Find("__System");
                if (parent == null)
                {
                    GameObject _System = new GameObject("__System");
                    DontDestroyOnLoad(_System);
                }
                go.transform.parent = parent.transform;
            }
        }
        return mInstance;
    }

    public void Destroy()
    {
        mInstance = null;
        UnityEngine.Object.Destroy(gameObject);
    }

}

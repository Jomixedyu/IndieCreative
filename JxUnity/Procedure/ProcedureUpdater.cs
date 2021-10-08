using System;
using UnityEngine;

internal class ProcedureUpdater : MonoBehaviour, IDisposable
{
    private static ProcedureUpdater instance;
    public static ProcedureUpdater GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(ProcedureUpdater)) as ProcedureUpdater;
            if (instance == null)
            {
                GameObject go = new GameObject($"__m_{nameof(ProcedureUpdater)}");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ProcedureUpdater>();
            }
        }
        return instance;
    }
    public static bool HasInstance()
    {
        return instance != null;
    }

    public void Dispose()
    {
        instance = null;
        Destroy(gameObject);
    }

    private void Update()
    {
        ProcedureManager.Update();
    }
}

using System;
using UnityEngine;

internal class ProcedureUpdater : Component
{
    private static ProcedureUpdater instance;
    public static ProcedureUpdater GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(ProcedureUpdater)) as ProcedureUpdater;
            if (instance == null)
            {
                GameObject go = new GameObject(typeof(ProcedureUpdater).Name);
                instance = go.AddComponent<ProcedureUpdater>();

                GameObject parent = GameObject.Find("__System");
                if (parent == null)
                {
                    parent = new GameObject("__System");
                    DontDestroyOnLoad(parent);
                }
                go.transform.parent = parent.transform;
            }
        }
        return instance;
    }

    private void Update()
    {
        ProcedureManager.OnTick();
    }
}

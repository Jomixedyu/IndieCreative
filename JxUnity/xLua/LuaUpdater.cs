using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class LuaUpdater : MonoBehaviour
{
    private static LuaUpdater instance;
    public static LuaUpdater GetInstance()
    {
        if (instance == null)
        {
            var go = new GameObject($"__m_{nameof(LuaUpdater)}");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<LuaUpdater>();
        }
        return instance;
    }
    public event Action OnUpdate;
    public event Action OnFixedUpdate;
    public event Action OnLateUpdate;

    private void Update()
    {
        OnUpdate.Invoke();
    }
    private void FixedUpdate()
    {
        OnFixedUpdate.Invoke();
    }
    private void LateUpdate()
    {
        OnLateUpdate.Invoke();
    }
}


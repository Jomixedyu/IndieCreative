using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updater : MonoSingleton<Updater>
{
    public event Action UpdateHandler;
    public event Action FixedUpdateHandler;
    public event Action LateUpdateHandler;

    private void Update()
    {
        UpdateHandler?.Invoke();
    }
    private void FixedUpdate()
    {
        FixedUpdateHandler?.Invoke();
    }
    private void LateUpdate()
    {
        LateUpdateHandler?.Invoke();
    }
}

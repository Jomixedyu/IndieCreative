using System;

public class Updater : MonoSingleton<Updater>
{
    public event Action UpdateHandler;
    public event Action FixedUpdateHandler;
    public event Action LateUpdateHandler;
    protected override void Awake()
    {
        if (CheckInstanceAndDestroy())
        {
            return;
        }
        base.Awake();
    }
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

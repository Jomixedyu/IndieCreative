using System;

/// <summary>
/// 使每个UI成为单例，通过绑定或添加，UIManager负责更新UI线程
/// </summary>
public class JuiManager : MonoSingleton<JuiManager>
{
    public event Action UpdateHandler;
    private void Update()
    {
        UpdateHandler?.Invoke();
    }
}

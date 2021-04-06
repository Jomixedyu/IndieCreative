using System;
using System.Collections.Generic;

/// <summary>
/// 使每个UI成为单例，通过绑定或添加，UIManager负责更新UI
/// </summary>
public class JuiManager : MonoSingleton<JuiManager>
{
    public event Action UpdateHandler;
    private List<Type> uiShowStack;
    private Dictionary<string, JuiBase> ui;

    private void Awake()
    {
        this.uiShowStack = new List<Type>();
        this.ui = new Dictionary<string, JuiBase>();
    }

    private void Update()
    {
        UpdateHandler?.Invoke();
    }

    public void Push(Type type)
    {
        this.uiShowStack.Add(type);
    }
    public Type Peek()
    {
        if(this.uiShowStack.Count == 0)
        {
            return null;
        }
        return this.uiShowStack[this.uiShowStack.Count - 1];
    }

    public void Pop(Type type)
    {
        this.uiShowStack.Remove(type);
    }

    public void AddUI(JuiBase ui)
    {
        this.ui.Add(ui.GetType().Name, ui);
    }

    public JuiBase GetUI(string name)
    {
        return this.ui[name];
    }
}

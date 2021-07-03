using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var item in types)
        {
            if (item.IsDefined(typeof(JuiPanelAttribute)))
            {
                if (item.GetCustomAttribute<JuiPanelAttribute>().IsPreBind)
                {
                    var method = item.BaseType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static);
                    method.Invoke(null, null);
                }
            }
        }
    }

    private void Update()
    {
        this.UpdateHandler?.Invoke();
    }

    public void Push(Type type)
    {
        this.uiShowStack.Add(type);
    }
    public void Pop(Type type)
    {
        this.uiShowStack.Remove(type);
    }

    /// <summary>
    /// 获取Focus
    /// </summary>
    /// <returns></returns>
    public Type GetFocus()
    {
        if (this.uiShowStack.Count == 0)
        {
            return null;
        }
        return this.uiShowStack[this.uiShowStack.Count - 1];
    }
    /// <summary>
    /// 设置为Focus
    /// </summary>
    /// <param name="type"></param>
    public void SetFocus(Type type)
    {
        this.uiShowStack.Remove(type);
        this.uiShowStack.Add(type);
    }

    public void AddUI(JuiBase ui)
    {
        this.ui.Add(ui.GetType().Name, ui);
    }
    public JuiBase GetUI(string name)
    {
        return this.ui[name];
    }

    public GameObject LoadResource(string path)
    {
        throw new System.NotImplementedException();
    }
}

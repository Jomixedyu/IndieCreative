using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 使每个UI成为单例，通过绑定或添加，UIManager负责更新UI
/// </summary>
public sealed class JuiManager : MonoSingleton<JuiManager>
{
    public event Action UpdateHandler;
    private List<Type> uiShowStack;
    private Dictionary<string, JuiBase> ui;

    private static Dictionary<string, UIInfo> uiTypes;
    private class UIInfo
    {
        public Type UIType;
        public string UIPath;
        public JuiPanelAttribute Attr;
    }
    private void AutoBind(UIInfo uiInfo)
    {
        if (uiInfo.Attr.IsPreBind && Instance.transform.Find(uiInfo.UIPath) != null)
        {
            Type genericType = typeof(JuiSingleton<>).MakeGenericType(new Type[] { uiInfo.UIType });
            var method = genericType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);
        }
    }
    private void Awake()
    {
        if (HasInstance)
        {
            //move
            if (transform.childCount > 0)
            {
                foreach (var item in uiTypes)
                {
                    string uiName = item.Key;
                    UIInfo uiInfo = item.Value;
                    if (Instance.Exist(uiName))
                    {
                        continue;
                    }
                    //move
                    Transform t = transform.Find(uiInfo.UIPath);
                    if (t != null)
                    {
                        t.SetParent(Instance.transform);
                        this.AutoBind(uiInfo);
                    }
                }
            }
            Destroy(this.gameObject);
            return;
        }

        this.uiShowStack = new List<Type>();
        this.ui = new Dictionary<string, JuiBase>();

        if (uiTypes == null)
        {
            uiTypes = new Dictionary<string, UIInfo>();
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.IsDefined(typeof(JuiPanelAttribute)))
                {
                    var attr = item.GetCustomAttribute<JuiPanelAttribute>();
                    string uiPath = attr.UiPath;
                    if (uiPath == null)
                    {
                        uiPath = item.Name;
                    }
                    uiTypes.Add(item.Name, new UIInfo() { UIType = item, UIPath = uiPath, Attr = attr });
                }
            }
        }

        //auto bind
        foreach (var item in uiTypes)
        {
            this.AutoBind(item.Value);
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
    public bool Exist(string ui)
    {
        return this.ui.ContainsKey(ui);
    }
    public GameObject LoadResource(string path)
    {
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }
        //unload all ui
        this.uiShowStack?.Clear();

        if (this.ui != null)
        {
            foreach (var item in this.ui)
            {
                item.Value.Dispose();
            }
            this.ui?.Clear();
        }

        base.Dispose();
    }
}

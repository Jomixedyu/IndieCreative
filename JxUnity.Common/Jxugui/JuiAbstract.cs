using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public abstract class JuiAbstract : IDisposable
{
    private GameObject _gameObject;
    private Transform _transform;
    public GameObject gameObject { get => _gameObject; }
    public Transform transform
    {
        get => _transform;
        protected set
        {
            _transform = value;
            if (_transform == null)
                _gameObject = null;
            else
                _gameObject = _transform.gameObject;
        }
    }

    private JuiPanelBaseAttribute attribute = null;
    protected JuiPanelBaseAttribute attr
    {
        get => attribute;
        set => attribute = value;
    }

    public string Name { get => attr.Name; }

    public virtual bool IsFocus
    {
        get => throw new NotImplementedException();
    }
    public virtual void SetFocus()
    {
        throw new NotImplementedException();
    }

    private bool isShow = false;
    public bool IsShow { get => isShow; }

    public virtual void Show()
    {
        if (this.isShow)
        {
            return;
        }
        _gameObject.SetActive(true);
        this.isShow = true;
        this.OnShow();
    }

    protected virtual void Update()
    {
        this.OnUpdate();
    }
    protected virtual void LogicHide()
    {
        this.isShow = false;
    }
    protected void InactiveHide()
    {
        this.OnHide();
        _gameObject.SetActive(false);
    }
    public virtual void Hide()
    {
        if (!this.isShow)
        {
            return;
        }
        this.isShow = false;
        InactiveHide();
    }

    public virtual void Create()
    {
        if (_transform == null)
        {
            var prefab = this.LoadResource(this.attribute.ResourcePath);
            var goinst = UnityEngine.Object.Instantiate(prefab, this.transform);
            goinst.name = this.Name;
            _transform = goinst.transform;
        }
        _gameObject = _transform.gameObject;
        this.isShow = _gameObject.activeSelf;

        if (this.attr.IsAutoBindElement)
        {
            this.AutoBindElement();
        }

        this.OnCreate();
        if (this.isShow)
        {
            this.OnShow();
        }
    }

    private void Destroy()
    {
        this.OnDestroy();

        UnityEngine.Object.Destroy(_gameObject);
        isShow = false;
        _gameObject = null;
        _transform = null;
    }

    protected object GetBindElementObject(Transform tran, Type type)
    {
        if (tran == null)
        {
            return null;
        }
        if (type.IsSubclassOf(typeof(Transform)))
        {
            return tran;
        }
        else if (type.IsSubclassOf(typeof(UnityEngine.Component)))
        {
            return tran.GetComponent(type);
        }
        else if (type.IsSubclassOf(typeof(GameObject)))
        {
            return tran.transform;
        }
        return null;
    }

    protected virtual void OnBindElement(List<FieldInfo> fields)
    {
        foreach (FieldInfo field in fields)
        {
            Type type = field.FieldType;
            if (field.IsDefined(typeof(JuiElementAttribute)))
            {
                JuiElementAttribute attr = field.GetCustomAttribute<JuiElementAttribute>();
                string path = attr.Path != null ? attr.Path : field.Name;

                Transform tran = transform.Find(path);
                if (tran == null)
                {
                    Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, field.Name));
                    continue;
                }

                object obj = GetBindElementObject(tran, type);
                field.SetValue(this, obj);

            }
            else if (field.IsDefined(typeof(JuiElementArrayAttribute)))
            {
                JuiElementArrayAttribute attr = field.GetCustomAttribute<JuiElementArrayAttribute>();

                string path = attr.Path != null ? attr.Path : field.Name;

                Transform tran = transform.Find(path);
                if (tran == null)
                {
                    Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, field.Name));
                    continue;
                }
                object fieldInst = null;

                if (type.IsArray)
                {
                    fieldInst = Array.CreateInstance(attr.ElementType, tran.childCount);
                    Array arr = (Array)fieldInst;
                    for (int i = 0; i < tran.childCount; i++)
                    {
                        object inst = GetBindElementObject(tran.GetChild(i), attr.ElementType);
                        arr.SetValue(inst, i);
                    }
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    fieldInst = Activator.CreateInstance(type);
                    IList list = (IList)fieldInst;
                    foreach (Transform childTransform in tran)
                    {
                        object inst = GetBindElementObject(childTransform, attr.ElementType);
                        list.Add(inst);
                    }
                }
                field.SetValue(this, fieldInst);
            }
        }
    }

    private void AutoBindElement()
    {
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        List<FieldInfo> infos = new List<FieldInfo>();
        foreach (FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(JuiAbstractAttribute), true))
            {
                infos.Add(field);
            }
        }
        this.OnBindElement(infos);
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="path"></param>
    protected virtual GameObject LoadResource(string path)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取子物体上的组件
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    protected TComponent GetComponentInChild<TComponent>(string path = "") where TComponent : Component
    {
        Transform f = transform;
        if (!string.IsNullOrEmpty(path))
            f = transform.Find(path);
        return f.GetComponent<TComponent>();
    }

    /// <summary>
    /// 显示隐藏切换
    /// </summary>
    public virtual void Switch()
    {
        if (this.isShow)
        {
            this.Hide();
        }
        else
        {
            this.Show();
        }
    }

    protected virtual void OnCreate() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDestroy() { }
    /// <summary>
    /// message
    /// </summary>
    public virtual void OnFocus() { }
    /// <summary>
    /// message
    /// </summary>
    public virtual void OnLostFocus() { }

    public virtual void Dispose()
    {
        this.Destroy();
    }

}
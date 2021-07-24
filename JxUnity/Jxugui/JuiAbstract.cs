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
        this.LogicHide();
        this.InactiveHide();
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
    }

    private void Destroy()
    {
        this.OnDestroy();

        UnityEngine.Object.Destroy(_gameObject);
        isShow = false;
        _gameObject = null;
        _transform = null;
    }


    protected static class BindUtil
    {
        public static object GetBindElementObject(Transform tran, Type type)
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
                return tran.gameObject;
            }
            return null;
        }
        public static bool IsFieldOrProp(MemberInfo info)
        {
            return info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property;
        }
        public static Type GetFieldOrPropType(MemberInfo info)
        {
            Type type = default;
            if (info.MemberType == MemberTypes.Field)
            {
                type = ((FieldInfo)info).FieldType;
            }
            else if (info.MemberType == MemberTypes.Property)
            {
                type = ((PropertyInfo)info).PropertyType;
            }
            return type;
        }
        public static void SetFieldOrPropValue(MemberInfo info, object inst, object value)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)info).SetValue(inst, value);
            }
            else if (info.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo)info).SetValue(inst, value);
            }
        }
    }

    private void BindElement(MemberInfo info, Type type)
    {
        JuiElementAttribute attr = info.GetCustomAttribute<JuiElementAttribute>();
        string path = attr.Path != null ? attr.Path : info.Name;

        Transform tran = transform.Find(path);
        if (tran == null)
        {
            Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, info.Name));
            return;
        }

        object obj = BindUtil.GetBindElementObject(tran, type);
        BindUtil.SetFieldOrPropValue(info, this, obj);
    }
    private void BindElementArray(MemberInfo info, Type type)
    {
        JuiElementArrayAttribute attr = info.GetCustomAttribute<JuiElementArrayAttribute>();

        string path = attr.Path != null ? attr.Path : info.Name;

        Transform tran = transform.Find(path);
        if (tran == null)
        {
            Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, info.Name));
            return;
        }
        object fieldInst = null;

        if (type.IsArray)
        {
            fieldInst = Array.CreateInstance(attr.ElementType, tran.childCount);
            Array arr = (Array)fieldInst;
            for (int i = 0; i < tran.childCount; i++)
            {
                object inst = BindUtil.GetBindElementObject(tran.GetChild(i), attr.ElementType);
                arr.SetValue(inst, i);
            }
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            fieldInst = Activator.CreateInstance(type);
            IList list = (IList)fieldInst;
            foreach (Transform childTransform in tran)
            {
                object inst = BindUtil.GetBindElementObject(childTransform, attr.ElementType);
                list.Add(inst);
            }
        }
        BindUtil.SetFieldOrPropValue(info, this, fieldInst);
    }

    protected virtual void OnBindElement(List<MemberInfo> fields)
    {
        foreach (MemberInfo info in fields)
        {
            if (BindUtil.IsFieldOrProp(info))
            {
                Type type = BindUtil.GetFieldOrPropType(info);

                if (info.IsDefined(typeof(JuiElementAttribute)))
                {
                    BindElement(info, type);
                }
                else if (info.IsDefined(typeof(JuiElementArrayAttribute)))
                {
                    BindElementArray(info, type);
                }
            }

        }
    }

    private void AutoBindElement()
    {
        var fields = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        List<MemberInfo> infos = new List<MemberInfo>();
        foreach (MemberInfo field in fields)
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

    public enum MessageType
    {
        Show, Hide, Update, Destroy, Focus, LostFocus
    }

    public void SendMessage(MessageType type)
    {
        switch (type)
        {
            case MessageType.Show: this.OnShow(); break;
            case MessageType.Hide: this.OnHide(); break;
            case MessageType.Update: this.OnUpdate(); break;
            case MessageType.Destroy: this.OnDestroy(); break;
            case MessageType.Focus: this.OnFocus(); break;
            case MessageType.LostFocus: this.OnLostFocus(); break;
            default:
                break;
        }
    }

    protected virtual void OnCreate() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDestroy() { }
    protected virtual void OnFocus() { }
    protected virtual void OnLostFocus() { }

    public virtual void Dispose()
    {
        this.Destroy();
    }

}
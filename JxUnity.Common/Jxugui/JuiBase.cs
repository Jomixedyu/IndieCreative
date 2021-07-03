using System;
using System.Collections;
using System.Reflection;
using UnityEngine;


public abstract class JuiBase
{
    private GameObject _gameObject;
    private Transform _transform;
    public GameObject gameObject { get => _gameObject; }
    public Transform transform { get => _transform; }

    private JuiPanelAttribute attribute = null;

    public bool IsFocus
    {
        get
        {
            return JuiManager.Instance.GetFocus() == this.GetType();
        }
    }

    private bool isShow = false;
    public bool IsShow { get => isShow; }

    private bool isNextFrame = false;

    public virtual void Show()
    {
        if (!this.isShow)
        {
            _gameObject.SetActive(true);
            this.isShow = true;

            JuiManager.Instance.Push(this.GetType());

            this.OnShow();
            this.isNextFrame = false;
            if (this.attribute.EnableUpdate)
            {
                JuiManager.Instance.UpdateHandler += Internel_OnUpdate;
            }
        }
    }
    //public void ShowDialog(JuiBase parent)
    //{
    //TODO
    //}
    private void Internel_OnUpdate()
    {
        if (!this.isNextFrame)
        {
            this.isNextFrame = true;
            return;
        }
        this.OnUpdate();
    }

    /// <summary>
    /// 逻辑上隐藏，但并不取消激活物体
    /// </summary>
    protected void SetHide()
    {
        if (this.isShow)
        {
            this.isShow = false;

            JuiManager.Instance.Pop(this.GetType());

            this.OnHide();
            if (this.attribute.EnableUpdate)
            {
                JuiManager.Instance.UpdateHandler -= Internel_OnUpdate;
            }
        }
    }
    protected void InactiveHide()
    {
        _gameObject.SetActive(false);
    }
    public virtual void Hide()
    {
        if (this.isShow)
        {
            this.SetHide();
            this.InactiveHide();
        }
    }

    public JuiBase()
    {
        object[] attrs = this.GetType().GetCustomAttributes(typeof(JuiPanelAttribute), false);
        if (attrs.Length == 0)
        {
            this.attribute = new JuiPanelAttribute();
        }
        else
        {
            this.attribute = attrs[0] as JuiPanelAttribute;
        }

        if (this.attribute.UiPath == null)
        {
            this.attribute.UiPath = this.GetType().Name;
        }

        this.Create();
    }

    public void Create()
    {
        //寻找场景内存在的游戏物体
        _transform = JuiManager.Instance.transform.Find(this.attribute.UiPath);
        if (_transform == null)
        {
            var prefab = this.LoadResource(this.attribute.ResourcePath);
            var goinst = UnityEngine.Object.Instantiate(prefab, this.transform);
            goinst.name = this.GetType().Name;
            _transform = goinst.transform;
        }
        _gameObject = _transform.gameObject;
        this.isShow = _gameObject.activeSelf;
        if (this.isShow)
        {
            JuiManager.Instance.Push(this.GetType());
            if (this.attribute.EnableUpdate)
            {
                JuiManager.Instance.UpdateHandler += Internel_OnUpdate;
            }
        }
        JuiManager.Instance.AddUI(this);

        //绑定元素
        this.AutoBindElement();

        this.OnCreate();
        if (this.isShow)
        {
            this.OnShow();
        }
    }
    
    public void Destroy()
    {
        this.OnDestroy();

        UnityEngine.Object.Destroy(_gameObject);

        _gameObject = null;
        _transform = null;
    }

    private object GetBindElementObject(Transform tran, Type type)
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
    private void AutoBindElement()
    {
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="path"></param>
    protected virtual GameObject LoadResource(string path)
    {
        return JuiManager.Instance.LoadResource(path);
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
}
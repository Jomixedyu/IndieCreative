using UnityEngine;


public abstract class JuiBase
{
    private GameObject _gameObject;
    private Transform _transform;
    public GameObject gameObject { get => _gameObject; }
    public Transform transform { get => _transform; }

    public abstract string uiPath { get; }
    public virtual string resPath { get; }
    /// <summary>
    /// 为true则面板可以被设置为Focus
    /// </summary>
    protected bool isDialogMode = true;
    public bool IsFocus
    {
        get
        {
            return JuiManager.Instance.Peek() == this.GetType();
        }
    }

    protected bool isShow = false;
    public bool IsShow { get => isShow; }

    private bool isNextFrame = false;

    public virtual void Show()
    {
        if (!isShow)
        {
            _gameObject.SetActive(true);
            isShow = true;

            if (isDialogMode)
            {
                JuiManager.Instance.Push(this.GetType());
            }
            
            OnShow();
            isNextFrame = false;
            JuiManager.Instance.UpdateHandler += Internel_OnUpdate;
        }
    }
    private void Internel_OnUpdate()
    {
        if (!isNextFrame)
        {
            isNextFrame = true;
            return;
        }
        this.OnUpdate();
    }

    /// <summary>
    /// 除了取消激活物体外的隐藏工作
    /// </summary>
    protected void SetHide()
    {
        if (isShow)
        {
            isShow = false;

            if (isDialogMode)
            {
                JuiManager.Instance.Pop(this.GetType());
            }

            OnHide();
            JuiManager.Instance.UpdateHandler -= Internel_OnUpdate;
        }
    }
    protected void InactiveHide()
    {
        _gameObject.SetActive(false);
    }
    public virtual void Hide()
    {
        if (isShow)
        {
            SetHide();
            InactiveHide();
        }
    }

    public JuiBase()
    {
        //寻找场景内存在的游戏物体
        _transform = JuiManager.Instance.transform.Find(uiPath);
        if(_transform == null)
        {
            _transform = Add(resPath);
            if (_transform == null)
                Debug.LogError("没有找到该UI节点: " + uiPath);
        }
        _gameObject = _transform.gameObject;
        isShow = _gameObject.activeSelf;
        //初始状态就显示直接添加事件
        this.OnCreate();
        if (isShow)
        {
            if (isDialogMode)
            {
                JuiManager.Instance.Push(this.GetType());
            }
            this.OnShow();
            JuiManager.Instance.UpdateHandler += OnUpdate;
        }
        JuiManager.Instance.AddUI(this);
    }
    /// <summary>
    /// 动态添加至场景，需要实现
    /// </summary>
    /// <param name="path"></param>
    protected Transform Add(string path)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 获取子物体上的组件
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    protected TComponent GetComponent<TComponent>(string path = "") where TComponent : Component
    {
        Transform f = transform;
        if (!string.IsNullOrEmpty(path))
            f = transform.Find(path);
        return f.GetComponent<TComponent>();
    }

    /// <summary>
    /// 显示隐藏switch切换
    /// </summary>
    public virtual void Switch()
    {
        if (isShow)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    protected virtual void OnCreate() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
}
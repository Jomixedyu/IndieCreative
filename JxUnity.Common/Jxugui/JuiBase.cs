using UnityEngine;

public abstract class JuiBase<UIType> : Singleton<UIType> where UIType : JuiBase<UIType>, new()
{
    private GameObject _gameObject;
    private Transform _transform;
    public GameObject gameObject { get => _gameObject; }
    public Transform transform { get => _transform; }

    public abstract string uiPath { get; }
    public virtual string resPath { get; }

    protected bool isShow = false;
    public bool IsShow { get => isShow; }
    public virtual void Show()
    {
        if (!isShow)
        {
            _gameObject.SetActive(true);
            isShow = true;
            OnShow();
            JUIManager.Instance.UpdateHandler += OnUpdate;
        }
    }
    public virtual void Hide()
    {
        if (isShow)
        {
            _gameObject.SetActive(false);
            isShow = false;
            OnHide();
            JUIManager.Instance.UpdateHandler -= OnUpdate;
        }
    }

    public JuiBase()
    {
        //寻找场景内存在的游戏物体
        _transform = JUIManager.Instance.transform.Find(uiPath);
        if(_transform == null)
        {
            _transform = Add(resPath);
            if (_transform == null)
                Debug.LogError("没有找到该UI节点: " + uiPath);
        }
        _gameObject = _transform.gameObject;
        isShow = _gameObject.activeSelf;
        //初始状态就显示直接添加事件
        if (isShow)
        {
            JUIManager.Instance.UpdateHandler += OnUpdate;
        }
        this.OnCreate();
        if (IsShow)
        {
            this.OnShow();
        }
    }
    /// <summary>
    /// 动态添加至场景，需要实现
    /// </summary>
    /// <param name="path"></param>
    protected Transform Add(string path)
    {
        return null;
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

    protected virtual void OnCreate() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
}
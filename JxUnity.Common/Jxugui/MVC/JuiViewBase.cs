using UnityEngine;

public abstract class JuiViewBase<TController, TModel, TView>
    where TController : JuiControllerBase<TController, TModel, TView>
    where TModel : JuiModelBase<TController, TModel, TView>
    where TView : JuiViewBase<TController, TModel, TView>
{
    private TController _mvc_controller = default;
    protected TController Controller { get => _mvc_controller; }

    private TModel _mvc_model = default;
    protected TModel Model { get => _mvc_model; }

    private Transform _transform = null;
    private GameObject _gameObject = null;
    protected Transform transform { get => _transform; }
    protected GameObject gameObject { get => _gameObject; }

    public virtual void OnRefresh()
    {

    }
}
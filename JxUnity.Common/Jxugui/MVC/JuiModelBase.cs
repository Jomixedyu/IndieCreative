

public abstract class JuiModelBase<TController, TModel, TView>
    where TController : JuiControllerBase<TController, TModel, TView>
    where TModel : JuiModelBase<TController, TModel, TView>
    where TView : JuiViewBase<TController, TModel, TView>
{
    private TController _mvc_controller = default;
    protected TController Controller { get => _mvc_controller; }

    private TView _mvc_view = default;
    protected TView View { get => _mvc_view; }

}
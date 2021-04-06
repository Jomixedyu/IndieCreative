using System;
using System.Reflection;

public class JuiControllerBase<TController, TModel, TView> 
    : JuiSingleton<JuiControllerBase<TController, TModel, TView>>
    where TController : JuiControllerBase<TController, TModel, TView>
    where TModel : JuiModelBase<TController, TModel, TView>
    where TView : JuiViewBase<TController, TModel, TView>
{
    private TModel _mvc_model;
    private TView _mvc_view;

    public override string uiPath => throw new NotImplementedException();

    protected TModel Model
    {
        get => _mvc_model;
        set
        {
            this._mvc_model = value;
            if (value == null)
            {
                return;
            }
            Type t = typeof(JuiModelBase<TController, TModel, TView>);
            t.GetField("_mvc_controller", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this);

            if (this.View != null)
            {
                t.GetField("_mvc_view", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this._mvc_view);
            }
        }
    }
    protected TView View
    {
        get => _mvc_view;
        set
        {
            this._mvc_view = value;
            if (value == null)
            {
                return;
            }
            Type t = typeof(JuiViewBase<TController, TModel, TView>);
            t.GetField("_mvc_controller", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this);
            t.GetField("_transform", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this.transform);
            t.GetField("_gameObject", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this.gameObject);

            if (this.Model != null)
            {
                t.GetField("_mvc_model", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, this._mvc_model);
            }
        }
    }

    protected void Init(TModel model, TView view)
    {
        this._mvc_model = model;
        this._mvc_view = view;
        this.Model = model;
        this.View = view;
    }

    protected void RefreshView()
    {
        this.View.OnRefresh();
    }

}

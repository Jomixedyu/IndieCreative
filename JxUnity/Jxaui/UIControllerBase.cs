using System;
using System.Collections.Generic;

public abstract class UIControllerBaseAbstract
{
    private UIViewBase view;
    protected UIViewBase View { get => view; }
    public UIControllerBaseAbstract(UIViewBase view)
    {
        this.view = view;
        this.view.Controller = this;
    }
    public void Open()
    {
        this.View.Open();
    }
    public void Close()
    {
        this.View.Close();
    }
}

public abstract class UIControllerBase<T> : UIControllerBaseAbstract where T : UIControllerBase<T>, new()
{
    private static UIControllerBase<T> instance;
    public static UIControllerBase<T> GetInstance()
    {
        if (instance == null)
        {
            instance = Activator.CreateInstance<T>();
        }
        return instance;
    }

    public UIControllerBase(UIViewBase view) : base(view) { }
}

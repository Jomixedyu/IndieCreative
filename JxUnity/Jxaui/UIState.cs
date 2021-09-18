using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIStateType
{
    Closed,
    Closing,
    Open,
    Loading,
}
public abstract class UIStateBase : FSMStateBase
{
    private UIViewBase view;
    public UIViewBase View { get => view; }
    public UIStateBase(UIViewBase view)
    {
        this.view = view;
    }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
}
public sealed class UIStateClosed : UIStateBase
{
    public UIStateClosed(UIViewBase view) : base(view) { }
}
public sealed class UIStateClosing : UIStateBase
{
    public UIStateClosing(UIViewBase view) : base(view) { }
}
public sealed class UIStateOpen : UIStateBase
{
    public UIStateOpen(UIViewBase view) : base(view) { }
}
public sealed class UIStateLoading : UIStateBase
{
    public UIStateLoading(UIViewBase view) : base(view) { }
}

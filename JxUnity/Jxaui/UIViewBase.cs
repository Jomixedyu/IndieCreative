using System;
using System.Collections.Generic;


public abstract class UIViewBase
{
    public UIControllerBaseAbstract Controller { get; set; }
    protected FSM<UIStateType, UIStateBase> fsm;

    public UIViewBase()
    {
        this.fsm = new FSM<UIStateType, UIStateBase>()
            .AddState(UIStateType.Closed, new UIStateClosed(this))
            .AddState(UIStateType.Closing, new UIStateClosing(this))
            .AddState(UIStateType.Loading, new UIStateLoading(this))
            .AddState(UIStateType.Open, new UIStateOpen(this));
    }

    public void Open()
    {
        this.fsm.GetCurState().OnOpen();
    }
    public void Close()
    {
        this.fsm.GetCurState().OnClose();
    }
    public virtual void OnUpdate() { }
}
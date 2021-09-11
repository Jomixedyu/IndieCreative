using System;
using System.Collections.Generic;


public abstract class FSMStateBase
{
    public object HostFsm { get; set; }
    public virtual void OnEnter() { }
    public virtual void OnLeave() { }
}

public class FSM<TFSMIndex, TFSMState> where TFSMState : FSMStateBase
{
    private IDictionary<TFSMIndex, TFSMState> fsm = new Dictionary<TFSMIndex, TFSMState>();

    private bool hasLastState = false;
    public TFSMIndex LastStateIndex { get; protected set; }

    private TFSMState curState = null;
    private TFSMIndex curIndex = default;

    public FSM<TFSMIndex, TFSMState> AddState(TFSMIndex fsmIndex, TFSMState state)
    {
        state.HostFsm = this;
        fsm.Add(fsmIndex, state);
        return this;
    }
    public void RemoveState(TFSMIndex fsmIndex)
    {
        fsm.Remove(fsmIndex);
    }
    public bool HasState(TFSMIndex fsmIndex)
    {
        return fsm.ContainsKey(fsmIndex);
    }
    public void ChangePreState()
    {
        if (!hasLastState)
        {
            return;
        }
        this.ChangeState(LastStateIndex);
    }
    public virtual void ChangeState(TFSMIndex fsmIndex)
    {
        if (curState != null)
        {
            curState.OnLeave();
            LastStateIndex = curIndex;
            hasLastState = true;
        }
        curIndex = fsmIndex;
        curState = fsm[fsmIndex];
        curState.OnEnter();
    }

    public TFSMState GetCurState()
    {
        return curState;
    }

    public void Reset()
    {
        if (curState != null)
        {
            curState.OnLeave();
        }
        LastStateIndex = default;
        curIndex = default;
        hasLastState = false;
        curState = null;
        fsm.Clear();
    }

}
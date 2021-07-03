using System;
using System.Collections.Generic;

public class FSMStateBase
{
    public virtual void OnEnter() { }
    public virtual void OnLeave() { }
}

public class FSMBase<TFSMIndex, TFSMState> where TFSMState : FSMStateBase
{
    private IDictionary<TFSMIndex, TFSMState> fsm = new Dictionary<TFSMIndex, TFSMState>();

    private bool hasPreState = false;
    public TFSMIndex preIndex { get; protected set; }

    private TFSMState curState = null;
    private TFSMIndex curIndex = default;

    public void AddState(TFSMIndex fsmIndex, TFSMState state)
    {
        fsm.Add(fsmIndex, state);
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
        if (!hasPreState)
        {
            return;
        }
        this.ChangeState(preIndex);
    }
    public virtual void ChangeState(TFSMIndex fsmIndex)
    {
        if (curState != null)
        {
            curState.OnLeave();
            preIndex = curIndex;
            hasPreState = true;
        }
        curIndex = fsmIndex;
        curState = fsm[fsmIndex];
        curState.OnEnter();
    }

    public TFSMState GetCurState()
    {
        return curState;
    }
}
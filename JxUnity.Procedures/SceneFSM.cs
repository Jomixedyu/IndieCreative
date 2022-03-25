using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneStateBase
{
    public virtual void OnEnter() { }
    public virtual void OnLeave() { }
    public virtual void OnUpdate() { }
}

public class SceneChangingState : SceneStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        //open
    }
    public override void OnLeave()
    {
        //close
    }
}

public class SceneFSM
{

}

internal class SceneFSMMono : MonoBehaviour
{
    private Dictionary<string, SceneStateBase> scenes;
    private string curState;
    private string prevState;

    private SceneStateBase changingState;
    private bool isChanging;
    public bool IsChangeable => !isChanging;

    private void Awake()
    {
        this.isChanging = false;
        this.scenes = new Dictionary<string, SceneStateBase>();
    }

    
    public void SetChangingState(SceneStateBase changingState)
    {
        this.changingState = changingState;
    }
    public SceneStateBase GetChangingState()
    {
        return this.changingState;
    }

    public void Change(string name)
    {
        if (this.isChanging)
        {
            return;
        }
        this.isChanging = true;

        this.prevState = this.curState;
        if (curState != null)
        {
            scenes[name].OnLeave();
        }

        if (this.changingState != null)
        {
            this.changingState.OnEnter();
        }
        StartCoroutine(LoadScene(name));
    }

    void OnChanged()
    {

    }

    IEnumerator LoadScene(string name)
    {
        var opr = SceneManager.LoadSceneAsync(name);
        opr.allowSceneActivation = false;
        while (opr.progress < 0.9f)
        {
            yield return null;
        }
        
    }

}
using System;
using System.Collections.Generic;
using UnityEngine;
using _ActorType = ActorAbstract;

public class ActorManager : MonoSingleton<ActorManager>
{
    private List<_ActorType> actors = new List<_ActorType>();
    private _ActorType main;
    public _ActorType Main { get => main; set => main = value; }

    public _ActorType MainTarget { get; set; }

    //private static Transform transform;
    //private static GameObject gameObject;

    private int idCount = 0;

    private int frameCount = 0;

    private void FixedUpdateHandler()
    {
        ++frameCount;
        foreach (_ActorType actor in actors)
        {
            actor.FixedUpdate(frameCount);
        }
    }

    private void UpdateHandler()
    {
        foreach (_ActorType actor in actors)
        {
            actor.Update();
        }
    }

    private void FixedUpdate()
    {
        FixedUpdateHandler();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Reset()
    {
        idCount = 0;
        main = null;
        MainTarget = null;
        actors.Clear();
    }
}

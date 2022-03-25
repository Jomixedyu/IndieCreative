using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorManagerAbstract<_ActorType> : MonoBehaviour where _ActorType : ActorAbstract
{
    private List<_ActorType> actors = new List<_ActorType>();
    private _ActorType main;
    public _ActorType Main { get => main; set => main = value; }

    public _ActorType MainTarget { get; set; }

    private int idCount = 0;

    private int frameCount = 0;

    private void FixedUpdateHandler()
    {
        ++frameCount;
        foreach (_ActorType actor in actors)
        {
            actor.FixedLogicUpdate(frameCount);
        }
    }

    private void UpdateHandler()
    {
        foreach (_ActorType actor in actors)
        {
            actor.RenderUpdate();
        }
    }

    private void FixedUpdate()
    {
        FixedUpdateHandler();
    }

    public void Reset()
    {
        idCount = 0;
        main = null;
        MainTarget = null;
        actors.Clear();
    }
}

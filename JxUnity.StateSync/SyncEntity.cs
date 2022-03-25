using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//客户端和服务端存在的逻辑实体
public class SyncEntity
{
    private int netId;
    private bool isDirty;

#if !SERVER
    private GameObject gameObject;
#endif
    protected virtual void OnTick() { }
    protected virtual void ModifyData() { }
    protected void Sync() { }
    
}

//客户端和服务端存在的逻辑实体
public class BlockEntity : SyncEntity
{

    protected override void OnTick()
    {
        base.OnTick();

    }
}
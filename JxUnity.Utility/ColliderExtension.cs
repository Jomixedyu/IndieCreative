using System;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderExtension
{
    /// <summary>
    /// 在当前碰撞触发器关系中的物体是否存在一个tag
    /// </summary>
    /// <param name="self"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool HasTagForCollider2DTrigger(this Transform self, string tag)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        self.GetComponent<Rigidbody2D>().OverlapCollider(new ContactFilter2D().NoFilter(), colliders);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 关闭该物体所有2D碰撞（包括子物体）
    /// </summary>
    /// <param name="self"></param>
    public static void DisableAllCollider2D(this Transform self)
    {
        foreach (Collider2D item in self.GetComponentsInChildren<Collider2D>())
        {
            item.enabled = false;
        }
    }
}

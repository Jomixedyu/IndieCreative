using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class TransformExtension
{
    /// <summary>
    /// 获取子物体中所有的组件（包括未激活过的物体）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static void GetComponentsInRawChildren<T>(Transform transform, ref List<T> list) where T : UnityEngine.Component
    {
        int count = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            //GetChild可以获取未激活过的物体
            var item = transform.GetChild(i);
            //子物体递归进入查找子物体
            GetComponentsInRawChildren(item, ref list);
        }
        T com = transform.GetComponent<T>();
        if (com != null) list.Add(com);
    }

    /// <summary>
    /// 获取子物体中所有的组件（包括未激活过的物体）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T[] GetComponentsInRawChildren<T>(this Transform transform) where T : UnityEngine.Component
    {
        List<T> rtn = new List<T>();
        GetComponentsInRawChildren(transform, ref rtn);
        return rtn.ToArray();
    }
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="component"></param>
    /// <param name="eventTriggerType"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static void AddListener(this Component component, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
    {
        EventTrigger eventTrigger = component.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = component.gameObject.AddComponent<EventTrigger>();
        }

        List<EventTrigger.Entry> triggers;
        if (eventTrigger.triggers != null)
            triggers = eventTrigger.triggers;
        else
            triggers = eventTrigger.triggers = new List<EventTrigger.Entry>();

        EventTrigger.Entry entry = null;
        foreach (EventTrigger.Entry item in triggers)
            if (item.eventID == eventTriggerType)
                entry = item;

        if (entry != null)
        {
            //存在直接添加
            entry.eventID = eventTriggerType;
            entry.callback.AddListener(callback);
        }
        else
        {
            //不存在
            entry = new EventTrigger.Entry();
            entry.eventID = eventTriggerType;
            entry.callback.AddListener(callback);
            triggers.Add(entry);
        }
    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="component"></param>
    /// <param name="eventTriggerType"></param>
    /// <param name="callback"></param>
    public static void RemoveListener(this Component component, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
    {
        EventTrigger eventTrigger = component.GetComponent<EventTrigger>();
        if (eventTrigger == null) return;

        if (eventTrigger.triggers == null) return;

        foreach (EventTrigger.Entry item in eventTrigger.triggers)
        {
            if (item.eventID == eventTriggerType)
            {
                item.callback.RemoveListener(callback);
                return;
            }
        }
    }
    /// <summary>
    /// 移除全部事件监听
    /// </summary>
    /// <param name="component"></param>
    /// <param name="eventTriggerType"></param>
    public static void RemoveAllListener(this Component component, EventTriggerType eventTriggerType)
    {
        EventTrigger eventTrigger = component.GetComponent<EventTrigger>();
        if (eventTrigger == null) return;

        if (eventTrigger.triggers == null) return;
        foreach (EventTrigger.Entry item in eventTrigger.triggers)
        {
            if (item.eventID == eventTriggerType)
            {
                item.callback.RemoveAllListeners();
                return;
            }
        }
    }

    /// <summary>
    /// 移除所有子物体
    /// </summary>
    /// <param name="component"></param>
    public static void RemoveChildren(this Component component)
    {
        Transform transform = component.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// 将子对象移至
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    public static void MoveChildrenTo(this Transform self, Transform target)
    {
        for (int i = 0; i < self.childCount; i++)
        {
            self.GetChild(0).SetParent(target);
        }
    }
    /// <summary>
    /// 按路径获取最后一个子对象
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetLastChildInPath(string path)
    {
        int pos = path.LastIndexOf('/');
        if (pos < 0)
        {
            return path;
        }
        return path.Substring(pos, path.Length - pos);
    }
    /// <summary>
    /// 按路径获取一个根名称
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRoot(string path)
    {
        int pos = path.IndexOf('/');
        if (pos < 0)
        {
            return path;
        }
        return path.Substring(0, pos + 1);
    }

    /// <summary>
    /// 查找一级激活子物体
    /// </summary>
    /// <param name="_this"></param>
    /// <returns></returns>
    public static int GetActiveChildCountInHierarchy(this Transform _this)
    {
        if (!_this.gameObject.activeInHierarchy)
        {
            return 0;
        }
        int count = 0;
        for (int i = 0; i < _this.childCount; i++)
        {
            if (_this.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// 获取第一个子对象
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform FirstChild(this Transform transform)
    {
        if (transform.childCount == 0)
        {
            return null;
        }
        return transform.GetChild(0);
    }
    /// <summary>
    /// 获取最后一个子对象
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform LastChild(this Transform transform)
    {
        int count = transform.childCount;
        if (count == 0)
        {
            return null;
        }
        return transform.GetChild(count - 1);
    }


    /// <summary>
    /// 创建子对象
    /// </summary>
    /// <param name="_this"></param>
    /// <param name="name"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static Transform CreateChild(this Transform _this, string name = "GameObject", Type[] components = null)
    {
        GameObject go;
        if (components == null)
        {
            go = new GameObject(name);
        }
        else
        {
            go = new GameObject(name, components);
        }
        go.layer = _this.gameObject.layer;
        go.transform.SetParent(_this, false);
        return go.transform;
    }

    public static Transform GetChild(this GameObject _this, int index)
    {
        return _this.transform.GetChild(index);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PreLoadCache : MonoSingleton<PreLoadCache>
{
    private Dictionary<string, List<GameObject>> caches;
    private void Awake()
    {
        this.caches = new Dictionary<string, List<GameObject>>();
    }
    public void Put(string type, GameObject go)
    {
        go.transform.SetParent(this.transform);
        go.SetActive(false);
        if (!this.caches.ContainsKey(type))
        {
            this.caches.Add(type, new List<GameObject>());
        }
        this.caches[type].Add(go);
    }
    public GameObject Get(string type)
    {
        GameObject go = null;
        if (this.caches.ContainsKey(type) && this.caches[type].Count > 0)
        {
            var list = this.caches[type];
            go = list[list.Count - 1];
            go.transform.SetParent(null);
            list.RemoveAt(list.Count - 1);
        }
        return go;
    }
    public void ReleaseAll()
    {
        this.caches.Clear();
        this.transform.RemoveChildren();
    }
}
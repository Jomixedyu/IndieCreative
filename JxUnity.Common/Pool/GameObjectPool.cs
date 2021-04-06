using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameObjectPool : MonoSingleton<GameObjectPool>
{
    private Dictionary<string, GameObject> proto;
    private Dictionary<string, Queue<GameObject>> pool;

    private void Awake()
    {
        this.proto = new Dictionary<string, GameObject>();
        this.pool = new Dictionary<string, Queue<GameObject>>();
    }

    public void Register(string type, GameObject gameObject, int initCount = 0)
    {
        this.proto.Add(type, gameObject);

        this.pool.Add(type, new Queue<GameObject>(initCount));

        for (int i = 0; i < initCount; i++)
        {
            this.pool[type].Enqueue(Instantiate(gameObject, this.transform));
        }
    }
    public void Recycle(string type, GameObject gameObject)
    {
        gameObject.transform.SetParent(this.transform);
        gameObject.SetActive(false);
        this.pool[type].Enqueue(gameObject);
    }
    public GameObject Get(string type)
    {
        if(this.pool[type].Count == 0)
        {
            return Instantiate(this.proto[type]);
        }
        return this.pool[type].Dequeue();
    }
}

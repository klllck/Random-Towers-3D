using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{

    [SerializeField] private List<GameObject> objectsToPool;
    private List<ObjectPoolEx<GameObject>> pools;

    public GameObject Get(string name)
    {
        if (pools == null)
        {
            pools = new List<ObjectPoolEx<GameObject>>();
        }

        var pool = pools.Find(p => p.objectToPool.name == name);

        if (pool == null)
        {
            pool = CreatePool(objectsToPool.First(obj => obj.name == name));
        }

        return pool.Get();
    }

    public void Release(GameObject gameObject)
    {
        var pool = pools.Find(p => p.objectToPool.name == gameObject.name);
        pool.Release(gameObject);
    }

    private ObjectPoolEx<GameObject> CreatePool(GameObject gameObject)
    {
        var pool = new ObjectPoolEx<GameObject>(gameObject,
           () =>
           {
               var obj = Instantiate(gameObject);
               obj.name = gameObject.name;
               return obj;
           },
           obj => { obj.SetActive(true); },
           obj => { obj.SetActive(false); },
           obj => { Destroy(obj); },
           false);

        pools.Add(pool);
        return pool;
    }
}

public class ObjectPoolEx<T> : ObjectPool<T> where T : class
{
    public T objectToPool;

    public ObjectPoolEx(T objectToPool,
        Func<T> createFunc,
        Action<T> actionOnGet = null,
        Action<T> actionOnRelease = null,
        Action<T> actionOnDestroy = null,
        bool collectionCheck = true,
        int defaultCapacity = 10,
        int maxSize = 10000)
        : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity)
    {
        this.objectToPool = objectToPool;
    }

    public void Expand(int count)
    {
        Stack<T> stack = new Stack<T>();

        for (int i = 0; i < count; i++)
        {
            stack.Push(Get());
        }
        for (int i = 0; i < count; i++)
        {
            Release(stack.Pop());
        }
    }
}

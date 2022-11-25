using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Transform objectPoolParent;

    private Queue<GameObject> objectPoolQueue;
    private int poolSize;
    private GameObject prefab;

    public void Init(int initSize, GameObject prefab, Transform objectPoolParent)
    {
        objectPoolQueue = new Queue<GameObject>();
        this.prefab = prefab;
        this.objectPoolParent = transform;

        for (var i = 0; i < initSize; i++) objectPoolQueue.Enqueue(CreateNewObject());
    }

    private GameObject CreateNewObject()
    {
        var newObj = Instantiate(prefab);
        newObj.transform.SetParent(objectPoolParent);
        newObj.SetActive(false);
        return newObj;
    }

    public GameObject GetObject()
    {
        if (objectPoolQueue.Count > 0)
        {
            var obj = objectPoolQueue.Dequeue();
            obj.transform.SetParent(objectPoolParent);
            obj.gameObject.SetActive(true);
            return obj;
        }

        var newObj = CreateNewObject();
        newObj.SetActive(true);
        return newObj;
    }

    public void RetrunPoolObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(objectPoolParent);
        objectPoolQueue.Enqueue(obj);
    }

    public int GetPoolSize()
    {
        return objectPoolQueue.Count;
    }
}
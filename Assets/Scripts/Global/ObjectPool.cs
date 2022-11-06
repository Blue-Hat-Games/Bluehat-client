using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjectPool : MonoBehaviour
{
    private int poolSize;

    private Queue<GameObject> objectPoolQueue;
    private GameObject prefab;

    private Transform objectPoolParent;

    public void Init(int initSize, GameObject prefab, Transform objectPoolParent)
    {
        objectPoolQueue = new Queue<GameObject>();
        this.prefab = prefab;
        this.objectPoolParent = this.transform;

        for(int i = 0; i < initSize; i++)
        {
            objectPoolQueue.Enqueue(CreateNewObject());
        }

    }

    private GameObject CreateNewObject()
    {
        var newObj = Instantiate(this.prefab);
        newObj.transform.SetParent(this.objectPoolParent);
        newObj.SetActive(false);
        return newObj;
    }

    public GameObject GetObject()
    {
        if(objectPoolQueue.Count > 0)
        {
            var obj = this.objectPoolQueue.Dequeue();
            obj.transform.SetParent(objectPoolParent);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = CreateNewObject();
            newObj.SetActive(true);
            return newObj; 
        }
    }

    public void RetrunPoolObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.objectPoolParent);
        this.objectPoolQueue.Enqueue(obj);
    }

    public int GetPoolSize()
    {
        return objectPoolQueue.Count;
    }
}
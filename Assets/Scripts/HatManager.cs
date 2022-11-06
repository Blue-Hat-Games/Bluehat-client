using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HatManager : MonoBehaviour
{
    public GameObject animalObject;

    public GameObject[] hatPrefabs;
    void Start()
    {
        Transform hatPoint = null;

        Transform[] allChildren = animalObject.GetComponentsInChildren<Transform>();
        foreach(Transform childTr in allChildren) 
        {
            if(childTr.name == "HatPoint")
            {
                hatPoint = childTr;   
            }
            
            Debug.Log(childTr.name);
        }
    
        SetHat(animalObject);

    }

    void SetHat(GameObject animalObj)
    {
        animalObj.SetActive(false);

        int initX = 0;
        for(int i = 0; i<hatPrefabs.Length; i++)
        {
            GameObject animal = Instantiate(animalObj, new Vector3(initX, 0, 0), Quaternion.identity);
            animal.SetActive(true);
            Transform hatPoint = null;
            Transform[] allChildren = animal.GetComponentsInChildren<Transform>();

            foreach(Transform childTr in allChildren) 
            {
                if(childTr.name == "HatPoint")
                {
                    hatPoint = childTr;   
                }
            }

            GameObject hatObj = Instantiate(hatPrefabs[i]);
            hatObj.transform.SetParent(hatPoint);
            hatObj.transform.localPosition = Vector3.zero;
            hatObj.transform.rotation = Quaternion.identity;

            initX += 3;
        }
    }
    
}

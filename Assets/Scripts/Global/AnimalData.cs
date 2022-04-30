using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalData : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class MainData
{
    public Animal[] animalList;
}

public class Animal
{
    public string email;
    public string name;
}
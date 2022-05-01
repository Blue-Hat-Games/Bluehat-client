using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    private void Awake() { 
        var obj = FindObjectsOfType<DataManager>(); 
        if (obj.Length == 1) { 
            DontDestroyOnLoad(gameObject); 
        } 
        else { 
            Destroy(gameObject); 
        } 
    }

    private AnimalListForSave animalListForSave = new AnimalListForSave();
    private void Start()
    {
        
        
    }

    public void AddNewAnimal(string animalName)
    {
        Debug.Log($"AddNewAnimal => {animalName}");
        animalListForSave.myAnimalList.Add(animalName);
    }

    public string GetAnimal()
    {
        string name = "Chick"; // 아무것도 없을 경우  임시값
       
        animalListForSave.myAnimalList.ForEach((string animalName) =>
        {
            name = animalName;
        });

        return name;
    }

}

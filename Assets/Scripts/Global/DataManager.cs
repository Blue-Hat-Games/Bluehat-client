using UnityEngine;

/* DataManager
    - Hold Animal List For Saving
*/
public class DataManager : MonoBehaviour
{
    private readonly AnimalListForSave animalListForSave = new();

    private void Awake()
    {
        var obj = FindObjectsOfType<DataManager>();
        if (obj.Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }

    public void AddNewAnimal(string animalName)
    {
        Debug.Log($"AddNewAnimal => {animalName}");
        animalListForSave.myAnimalList.Add(animalName);
    }

    public string GetAnimal()
    {
        var name = "Chick"; // 아무것도 없을 경우 임시값

        animalListForSave.myAnimalList.ForEach(animalName => { name = animalName; });

        return name;
    }
}
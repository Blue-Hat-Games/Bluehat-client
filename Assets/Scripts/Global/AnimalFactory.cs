using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnimalFactory : MonoBehaviour
{
    public Texture2D formatTexture;
    private string pngSavePath = "";

    // �������� ���� �����͸� �޾Ƽ� �ٽ� ������ ����
    [System.Serializable]
    public class AnimalDataFromServer
    {
        public string name;
        public int tier;
        public string color;
        public string id;
        public string animalType;
        public string headItem;
        public string pattern;
    }

    void Start()
    {
        pngSavePath = $"{Application.dataPath}/RuntimeImages";
    }

    public List<GameObject> ConvertJsonToAnimalObject(string jsonData)
    {
        List<GameObject> animalObjects = new List<GameObject>();

        Animal[] animalList = GetAnimalDataFromJson(jsonData);
        foreach (Animal data in animalList)
        {
            GameObject animalObj = GetAnimalGameObject(data);
            animalObjects.Add(animalObj);
        }

        return animalObjects;
    }

    private Animal[] GetAnimalDataFromJson(string txt)
    {
        var animalData = JsonHelper.FromJson<AnimalDataFromServer>(txt);
        Debug.Log($"animalData = {animalData.Length}");
        Animal[] animalList = new Animal[animalData.Length];

        for (int i = 0; i < animalData.Length; i++)
        {
            Animal animal = new Animal(animalData[i].name, animalData[i].tier, animalData[i].id, animalData[i].animalType, animalData[i].headItem, animalData[i].pattern, formatTexture);
            string color = animalData[i].color;
            color = color.Replace("\\", string.Empty);
            animal.setAnimalColor(color);
            animalList[i] = animal;
        }

        return animalList;
    }

    GameObject GetAnimalGameObject(Animal animalData)
    {
        Debug.Log($"type = {animalData.animalType}");
        GameObject animalPrefab = animalData.getAnimalPrefab();

        GameObject animalObj = GameObject.Instantiate(animalPrefab);
        animalObj.transform.LookAt(Camera.main.transform);
        animalObj.name = $"{animalData.animalType}_{animalData.id}";

        Texture2D meshTex = animalData.getAnimalTexture();
        animalObj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);

        return animalObj;
    }

}
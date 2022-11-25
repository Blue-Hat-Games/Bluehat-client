using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AnimalData
{
    private static string SavePath => Application.persistentDataPath + "/saves/";


    public static void Save(AnimalListForSave saveData, string saveFileName)
    {
        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

        var animalSaveData = new AnimalListForSave();

        var saveJson = JsonUtility.ToJson(animalSaveData);

        var saveFilePath = SavePath + saveFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }


    public static AnimalListForSave Load(string saveFileName)
    {
        var saveFilePath = SavePath + saveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        var saveFile = File.ReadAllText(saveFilePath);
        var saveData = JsonUtility.FromJson<AnimalListForSave>(saveFile);
        return saveData;
    }
}

[Serializable]
public class AnimalListForSave
{
    public List<string> myAnimalList = new();
}
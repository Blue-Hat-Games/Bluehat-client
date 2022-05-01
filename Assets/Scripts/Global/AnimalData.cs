using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AnimalData 
{
    private static string SavePath => Application.persistentDataPath + "/saves/";
	
	
	public static void Save(AnimalListForSave saveData, string saveFileName)
	{
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}

		AnimalListForSave animalSaveData = new AnimalListForSave();
		

		
		string saveJson = JsonUtility.ToJson(animalSaveData);

		string saveFilePath = SavePath + saveFileName + ".json";
		File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("Save Success: " + saveFilePath);
	}


	public static AnimalListForSave Load(string saveFileName)
	{
		string saveFilePath = SavePath + saveFileName + ".json";

		if (!File.Exists(saveFilePath))
		{
			Debug.LogError("No such saveFile exists");
			return null;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		AnimalListForSave saveData = JsonUtility.FromJson<AnimalListForSave>(saveFile);
		return saveData;
	}
}

[Serializable]
public class AnimalListForSave
{
    public List<string> myAnimalList = new List<string>();
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]
public class SaveData
{
	public SaveData(string _email, string _wallet_address)
	{
		email = _email;
		wallet_address = _wallet_address;
	}

	public string email;
	public string wallet_address;

}

public static class SaveSystem
{
	private static string SavePath => Application.persistentDataPath + "/saves/";

	public static void Save(SaveData saveData, string saveFileName)
	{
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}

		string saveJson = JsonUtility.ToJson(saveData);

		string saveFilePath = SavePath + saveFileName + ".json";
		File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("Save Success: " + saveFilePath);
	}

	public static SaveData Load(string saveFileName)
	{
		string saveFilePath = SavePath + saveFileName + ".json";

		if (!File.Exists(saveFilePath))
		{
			Debug.LogError("No such saveFile exists");
			return null;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
		return saveData;
	}
}
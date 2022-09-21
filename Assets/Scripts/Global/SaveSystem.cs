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
	private static string UserInfoSaveFileName = "userInfo";

	public static void SaveUserInfoFile(SaveData saveData)
	{
		if (false == Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
			Debug.Log($"SaveSystem | Create Directory => {SavePath}");
		}

		string saveJson = JsonUtility.ToJson(saveData);

		string saveFilePath = SavePath + UserInfoSaveFileName + ".json";
		File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("SaveUserInfoFile Success: " + saveFilePath);
	}

	public static SaveData LoadUserInfoFile()
	{
		string saveFilePath = SavePath + UserInfoSaveFileName + ".json";

		if (!File.Exists(saveFilePath))
		{
			Debug.Log("No such saveFile exists");
			return null;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
		return saveData;
	}

	public static void DeleteUserInfoFile()
	{
		string saveFilePath = SavePath + UserInfoSaveFileName + ".json";
		if(!File.Exists(saveFilePath))
		{
			Debug.Log("No such saveFile exists");
			return;
		}
		File.Delete(saveFilePath);
	}
}
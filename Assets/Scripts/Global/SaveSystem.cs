using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string email;

    public SaveData(string _email)
    {
        email = _email;
    }
}

public static class SaveSystem
{
    private static readonly string UserInfoSaveFileName = "userInfo";
    private static string SavePath => Application.persistentDataPath + "/saves/";

    public static void SaveUserInfoFile(SaveData saveData)
    {
        if (false == Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
            Debug.Log($"SaveSystem | Create Directory => {SavePath}");
        }

        var saveJson = JsonUtility.ToJson(saveData);

        var saveFilePath = SavePath + UserInfoSaveFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("SaveUserInfoFile Success: " + saveFilePath);
    }

    public static SaveData LoadUserInfoFile()
    {
        var saveFilePath = SavePath + UserInfoSaveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No such saveFile exists");
            return null;
        }

        var saveFile = File.ReadAllText(saveFilePath);
        var saveData = JsonUtility.FromJson<SaveData>(saveFile);
        return saveData;
    }

    public static void DeleteUserInfoFile()
    {
        var saveFilePath = SavePath + UserInfoSaveFileName + ".json";
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No such saveFile exists");
            return;
        }

        File.Delete(saveFilePath);
    }
}
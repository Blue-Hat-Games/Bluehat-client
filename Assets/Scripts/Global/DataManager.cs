// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;

// [System.Serializable]
// public class SaveData
// {
//     public List<string> testDataA = new List<string>();
//     public List<int> testDataB = new List<int>();
//     public int gold;
//     public int power;
// }
// public class DataManager : MonoBehaviour
// {
//     string path;
//     void Start()
//     {
//         path = Path.Combine(Application.dataPath, "database.json");
//         JsonLoad();
//     }
//     public void JsonLoad()
//     {
//         SaveData saveData = new SaveData();
//         if (!File.Exists(path))
//         {
//             GameManager.instance.playerGold = 100;
//             GameManager.instance.playerPower = 4;
//             SaveData();
//         }
//         else
//         {
//             string loadJson = File.ReadAllText(path);
//             saveData = JsonUtility.FromJson<SaveData>(loadJson);
//             if (saveData != null)
//             {
//                 for (int i = 0; i < saveData.testDataA.Count; i++)
//                 {
//                     GameManager.instance.testDataA.Add(saveData.testDataA[i]);
//                 }
//                 for (int i = 0; i < saveData.testDataB.Count; i++)
//                 { 
//                     GameManager.instance.testDataB.Add(saveData.testDataB[i]); 
//                 }
//                 GameManager.instance.playerGold = saveData.gold;
//                 GameManager.instance.playerPower = saveData.power;
//             }
//         }
//     }
//     public void JsonSave()
//     {
//         SaveData saveData = new SaveData();
//         for (int i = 0; i < 10; i++)
//         {
//             saveData.testDataA.Add("테스트 데이터 no " + i);
//         }
//         for (int i = 0; i < 10; i++)
//         {
//             saveData.testDataB.Add(i);
//         }
//         saveData.gold = GameManager.instance.playerGold;
//         saveData.power = GameManager.instance.playerPower;
//         string json = JsonUtility.ToJson(saveData, true);
//         File.WriteAllText(path, json);
//     }
// }

// // // string path = Path.Combine(Application.dataPath, "database.json"); // 불러오기
// // string loadJson = File.ReadAllText(path);
// // saveData = JsonUtility.FromJson<SaveData>(loadJson); // 저장하기 
// // string json = JsonUtility.ToJson(saveData); File.WriteAllText(path, json);

// // // 출처: https://geojun.tistory.com/65 [Jungle(정글)]

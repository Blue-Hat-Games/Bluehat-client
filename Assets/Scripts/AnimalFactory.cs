using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnimalFactory : MonoBehaviour
{
    public Texture2D formatTexture;
    private string animalPrefabPath = "Prefab/Animals";
    private string pngSavePath = "";

    // 서버에서 동물 데이터를 받아서 다시 동물로 조립
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

        AnimalDataFromServer[] animalDatas = GetAnimalDataFromJson(jsonData);
        foreach (AnimalDataFromServer data in animalDatas)
        {
            GameObject animalObj = GetAnimalGameObject(data);
            animalObjects.Add(animalObj);
        }

        return animalObjects;
    }

    private AnimalDataFromServer[] GetAnimalDataFromJson(string txt)
    {
        var animalData = JsonHelper.FromJson<AnimalDataFromServer>(txt);
        Debug.Log($"animalData = {animalData.Length}");

        for(int i=0; i < animalData.Length; i++)
        {
            Debug.Log($"name = {animalData[i].name}, tier = {animalData[i].tier}, animalType = {animalData[i].animalType}");
            Debug.Log($"color = {animalData[i].color}");
            // Color 값을 통해 텍스처 저장
            string color = animalData[i].color;
            color = color.Replace("\\", string.Empty);

            SaveJsonColorToTexture(color, animalData[i].animalType, animalData[i].id);
        }              

        return animalData;
    }

    GameObject GetAnimalGameObject(AnimalDataFromServer animalData)
    {
        Debug.Log($"type = {animalData.animalType}");
        string path = $"{animalPrefabPath}/{animalData.animalType}";
        GameObject animalPrefab = Resources.Load<GameObject>(path);

        GameObject animalObj = GameObject.Instantiate(animalPrefab);
        animalObj.transform.LookAt(Camera.main.transform);
        animalObj.name = $"{animalData.animalType}_{animalData.id}";

        // 텍스처 변경
        Texture2D meshTex = GetTexture(animalData.animalType, animalData.id);        
        animalObj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);

        return animalObj;
    }

    private void SaveJsonColorToTexture(string colorJsonStr, string animalType, string id)
    {
        colorJsonStr = "{\"data\":" + colorJsonStr;
        colorJsonStr = colorJsonStr + "}";
        Color32[] pix = formatTexture.GetPixels32();
        int originColor32Length = pix.Length;
    
        Debug.Log($"SaveJsonColorToTexture | {colorJsonStr}");

        // 22.9.18 json string을 다시 텍스처로 변환하는 테스트
        Color32[] colorFromJson = JsonHelper.FromJson<Color32>(colorJsonStr);
        for(int i = 0; i < colorFromJson.Length; i++)
        {
            Debug.Log($"{i} | pix = {colorFromJson[i]}");
        }

        // 다시 복원할 때
        Color32[] restoreTexColors = new Color32[originColor32Length];
        int index = 0;
        for(int i=0; i<restoreTexColors.Length; i++)
        {
            // 두번째 줄은 다시 처음으로 
            if(i == 16)
            {
                index = 0;
            }
            if(i == 48)
            {
                index = 8;
            }

            restoreTexColors[i] = colorFromJson[index];

            // 두칸마다 색 바꿔야 함 
            if(i % 2 != 0)
            {
                index ++;
            }
        }
   
        Texture2D restoreTexture = new Texture2D(formatTexture.width, formatTexture.height);

        restoreTexture.SetPixels32(restoreTexColors);
        restoreTexture.Apply(true);

        byte[] bytes = restoreTexture.EncodeToPNG();

        if(pngSavePath == "")
        {
            pngSavePath = $"{Application.dataPath}/RuntimeImages";        
        }

        if(!Directory.Exists(pngSavePath)) {
            Directory.CreateDirectory(pngSavePath);
        }

        File.WriteAllBytes($"{pngSavePath}/{animalType}_{id}.png", bytes);
    }

    private Texture2D GetTexture(string animalType, string id)
    {
        DirectoryInfo dir = new DirectoryInfo(pngSavePath);
        FileInfo[] files = dir.GetFiles($"{animalType}_{id}.png"); 

        Texture2D tex = new Texture2D(formatTexture.width, formatTexture.height);

        foreach(FileInfo file in files)
        {
            Debug.Log($"GetTexture | file.Name = {file.Name}");
            FileStream fs = new FileStream($"{pngSavePath}/{file.Name}", FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            tex.LoadImage(buffer);
            tex.Apply();
        }
        return tex;
    }
    
}


/*
{
    "data": [
        {
            "name": "testAnimal",
            "tier": 1,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1296,
            "animalType": "SeaLion",
            "headItem": "None",
            "pattern": "None"
        },
        {
            "name": "testAnimal",
            "tier": 1,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1297,
            "animalType": "Pig",
            "headItem": "None",
            "pattern": "None"
        },
        {
            "name": "Hamilton Drake",
            "tier": 2,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1298,
            "animalType": "Eagle",
            "headItem": "item3",
            "pattern": "pattern1"
        },
        {
            "name": "Hamilton Drake",
            "tier": 2,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1299,
            "animalType": "Hyena",
            "headItem": "item5",
            "pattern": "pattern1"
        },
        {
            "name": "George Oneil",
            "tier": 3,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1300,
            "animalType": "Eagle",
            "headItem": "item3",
            "pattern": "pattern7"
        },
        {
            "name": "Hamilton Drake",
            "tier": 2,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1301,
            "animalType": "Hyena",
            "headItem": "item5",
            "pattern": "pattern1"
        },
        {
            "name": "Hamilton Drake",
            "tier": 2,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":166,\"g\":134,\"b\":117,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":251,\"g\":200,\"b\":32,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1302,
            "animalType": "Hyena",
            "headItem": "item5",
            "pattern": "pattern1"
        },
        {
            "name": "George Oneil",
            "tier": 3,
            "color": "[{\"r\":233,\"g\":209,\"b\":151,\"a\":255},{\"r\":183,\"g\":145,\"b\":87,\"a\":255},{\"r\":77,\"g\":13,\"b\":234,\"a\":255},{\"r\":23,\"g\":208,\"b\":182,\"a\":255},{\"r\":247,\"g\":145,\"b\":64,\"a\":255},{\"r\":151,\"g\":246,\"b\":47,\"a\":255},{\"r\":0,\"g\":233,\"b\":198,\"a\":255},{\"r\":198,\"g\":145,\"b\":0,\"a\":255},{\"r\":233,\"g\":209,\"b\":151,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":19,\"g\":206,\"b\":182,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":244,\"g\":244,\"b\":244,\"a\":255},{\"r\":84,\"g\":84,\"b\":106,\"a\":255}]",
            "id": 1303,
            "animalType": "Hyena",
            "headItem": "item3",
            "pattern": "pattern1"
        },
        {
            "name": "Hamilton Drake",
            "tier": 3,
            "color": "[{\"r\":244,\"g\":232,\"b\":203,\"a\":255},{\"r\":219,\"g\":200,\"b\":171,\"a\":255},{\"r\":77,\"g\":13,\"b\":234,\"a\":255},{\"r\":139,\"g\":104,\"b\":91,\"a\":255},{\"r\":247,\"g\":145,\"b\":64,\"a\":255},{\"r\":203,\"g\":123,\"b\":151,\"a\":255},{\"r\":255,\"g\":244,\"b\":99,\"a\":255},{\"r\":99,\"g\":200,\"b\":255,\"a\":255},{\"r\":233,\"g\":209,\"b\":151,\"a\":255},{\"r\":183,\"g\":145,\"b\":87,\"a\":255},{\"r\":167,\"g\":134,\"b\":116,\"a\":255},{\"r\":137,\"g\":103,\"b\":91,\"a\":255},{\"r\":255,\"g\":207,\"b\":19,\"a\":255},{\"r\":71,\"g\":123,\"b\":132,\"a\":255},{\"r\":233,\"g\":233,\"b\":233,\"a\":255},{\"r\":42,\"g\":42,\"b\":53,\"a\":255}]",
            "id": 1304,
            "animalType": "Eagle",
            "headItem": "item3",
            "pattern": "pattern1"
        }
    ]
}
*/
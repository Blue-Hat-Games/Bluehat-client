using System;
using System.Collections.Generic;
using UnityEngine;

namespace BluehatGames
{
    public class AnimalFactory : MonoBehaviour
    {
        private string pngSavePath = "";

        private void Start()
        {
            pngSavePath = $"{Application.dataPath}/RuntimeImages";
        }

        public Dictionary<string, GameObject> ConvertJsonToAnimalObject(string jsonData)
        {
            var animalObjectDictionary = new Dictionary<string, GameObject>();

            var animalList = GetAnimalDataFromJson(jsonData);
            foreach (var data in animalList)
            {
                var animalObj = GetAnimalGameObject(data);
                animalObjectDictionary.Add(data.id, animalObj);
            }

            return animalObjectDictionary;
        }


        private Animal[] GetAnimalDataFromJson(string txt)
        {
            Debug.Log("AnimalFactory" + txt);
            var animalData = JsonHelper.FromJson<AnimalDataFormat>(txt);
            Debug.Log($"animalData = {animalData.Length}");
            var animalList = new Animal[animalData.Length];
            Debug.Log(animalData.ToString());
            for (var i = 0; i < animalData.Length; i++)
            {
                var animal = new Animal(animalData[i]);
                animalList[i] = animal;
            }

            return animalList;
        }

        public void ChangeTextureAnimalObject(GameObject animalObj, Animal animalData)
        {
            var meshTex = animalData.getAnimalTexture();
            animalObj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);
        }

        // synthesis manager 같은 곳에서 하나의 동물을 불러올 때 사용 
        public GameObject GetAnimalGameObject(Animal animalData)
        {
            Debug.Log($"type = {animalData.animalType}");
            if (animalData.animalType == null) return null;
            var animalPrefab = animalData.getAnimalPrefab();

            var animalObj = Instantiate(animalPrefab);
            animalObj.name = $"{animalData.animalType}_{animalData.id}";

            LoadHatItemPrefab(animalData.headItem, animalObj);
            var meshTex = animalData.getAnimalTexture();
            animalObj.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);

            return animalObj;
        }

        public void LoadHatItemPrefab(string itemName, GameObject animalObject)
        {
            Debug.Log($"LoadHatItemPrefab() | itemName = {itemName}");
            if (itemName is "None" or "") return;

            var path = $"Prefab/Hats/{itemName}";
            var obj = Resources.Load(path) as GameObject;
            var hatObj = Instantiate(obj);
            var allChildren = animalObject.GetComponentsInChildren<Transform>();
            Transform hatPoint = null;

            foreach (var childTr in allChildren)
                if (childTr.name == "HatPoint")
                    hatPoint = childTr;
            // 모자가 이미 있으면 삭제 
            if (hatPoint.childCount > 0) Destroy(hatPoint.GetChild(0).gameObject);

            hatObj.transform.SetParent(hatPoint);
            hatObj.transform.localPosition = Vector3.zero;
            hatObj.transform.localEulerAngles = Vector3.zero;
        }

        [Serializable]
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
    }
}
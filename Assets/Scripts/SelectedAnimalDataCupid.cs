using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BluehatGames 
{
    public class SelectedAnimalDataCupid : MonoBehaviour
    {
        private SelectedAnimalDataCupid instance;
        private AnimalDataFormat selectedAnimalData;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public void SetSelectedAnimalData(AnimalDataFormat animalData)
        {
            selectedAnimalData = animalData;
        }

        public AnimalDataFormat GetSelectedAnimalData()
        {
            return selectedAnimalData;
        }

        public string GetSelectedAnimalType()
        {
            return selectedAnimalData.animalType;
        }

        public void SetAnimalTexture(GameObject animalObject)
        {
            // 업데이트 할 동물의 오브젝트를 딕셔너리에서 가져옴
            Animal animal = new Animal(selectedAnimalData);
            // animal의 텍스처 변경
            Texture2D meshTex = animal.getAnimalTexture();
            animalObject.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);
        }

        public void SetRemoteAnimalTexture(AnimalDataFormat animalData, GameObject animalObject)
        {
            Animal animal = new Animal(animalData);
            // animal의 텍스처 변경
            Texture2D meshTex = animal.getAnimalTexture();
            animalObject.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);
        }

        public void SetHatObject(GameObject animalObject)
        {
            LoadHatItemPrefab(selectedAnimalData.headItem, animalObject);
        }
        
        public void LoadHatItemPrefab(string itemName, GameObject animalObject)
        {
            Debug.Log($"LoadHatItemPrefab() | itemName = {itemName}");
            if(itemName == "None" || itemName == "")
            {
                return;
            }

            var path = $"Prefab/Hats/{itemName}";
            GameObject obj = Resources.Load(path) as GameObject;
            GameObject hatObj = Instantiate(obj);
            Transform[] allChildren = animalObject.GetComponentsInChildren<Transform>();
            Transform hatPoint = null;

            foreach(Transform childTr in allChildren) 
            {
                if(childTr.name == "HatPoint")
                {
                    hatPoint = childTr;   
                }
            }
            // 모자가 이미 있으면 삭제 
            if(hatPoint.childCount > 0)
            {
                Destroy(hatPoint.GetChild(0).gameObject); 
            }
            
            hatObj.transform.SetParent(hatPoint, true);
            hatObj.transform.localPosition = Vector3.zero;
            hatObj.transform.localEulerAngles = Vector3.zero;
        }
    }
}

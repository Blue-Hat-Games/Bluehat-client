using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

namespace BluehatGames
{

    // 서버에서 동물 정보를 받아옴
    public class AnimalAirController : MonoBehaviour
    {
        public AnimalFactory animalFactory;

        public string tempAccessToken = "0000";
        private Dictionary<string, GameObject> animalObjectDictionary;
        private AnimalDataFormat[] prevAnimalDataArray;
        private AnimalDataFormat[] animalDataArray;

        private Scene currentScene;
        private string currentSceneName;


        void Start()
        {
            animalObjectDictionary = new Dictionary<string, GameObject>();
            currentScene = SceneManager.GetActiveScene();
            currentSceneName = currentScene.name;

            StartCoroutine(DownLoadGet(ApiUrl.getUserAnimal));
        }

        public IEnumerator DownLoadGet(string URL)
        {
            UnityWebRequest request = UnityWebRequest.Get(URL);
            var access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            // TODO: 임시로 설정
            access_token = tempAccessToken;

            Debug.Log($"access token = {access_token}");
            request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string jsonData = request.downloadHandler.text;
                switch (currentSceneName)
                {
                    case SceneName._03_Main:
                        SetMainSceneAnimals(jsonData);
                        break;
                    case SceneName._04_Synthesis:
                        SetSynthesisSceneAnimals(jsonData);
                        break;

                }
            }
        }

        private void SetMainSceneAnimals(string jsonData)
        {
            // json data를 넘기면 그 데이터를 통해 생성된 동물 오브젝트 리스트를 반환 받을 수 있다
            animalObjectDictionary = animalFactory.ConvertJsonToAnimalObject(jsonData);

            // 메인 씬에 동물 배치
            foreach (KeyValuePair<string, GameObject> pair in animalObjectDictionary)
            {
                GameObject animalObject = pair.Value;
                float randomX = UnityEngine.Random.Range(-20, 20);
                float randomZ = UnityEngine.Random.Range(-20, 20);
                animalObject.transform.position = new Vector3(randomX, 0.1f, randomZ);
                animalObject.transform.rotation = Quaternion.identity;

                animalObject.AddComponent<MainSceneAnimal>();
            }

        }

        private void SetSynthesisSceneAnimals(string jsonData)
        {
            animalObjectDictionary.Clear();
            animalObjectDictionary = animalFactory.ConvertJsonToAnimalObject(jsonData);
            // isRefresh가 true이면 여기에서 이전 jsonData랑 비교해서 달라진 오브젝트만 dictionary 교체해주자
            animalDataArray = JsonHelper.FromJson<AnimalDataFormat>(jsonData);
       
            GameObject.FindObjectOfType<SynthesisManager>().StartMakeThumbnailAnimalList(animalObjectDictionary, animalDataArray);
        }

       
        // 색 변경 이후 다시 데이터를 불러와야 함 
        public void RefreshAnimalDataColorChange(string animalId)
        {
            StartCoroutine(UpdateDataOnColorChange(ApiUrl.getUserAnimal, animalId));
        }

        private IEnumerator UpdateDataOnColorChange(string URL, string animalId)
        {   
            UnityWebRequest request = UnityWebRequest.Get(URL);
            var access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            // TODO: 임시로 설정
            access_token = tempAccessToken;

            Debug.Log($"access token = {access_token}");
            request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string jsonData = request.downloadHandler.text;
                SetUpdatedAnimalOnColorChange(animalId, jsonData);
            }
        }

        // 색 변경 이후 업데이트 된 동물에 한해 딕셔너리 데이터를 교체해주자
        private void SetUpdatedAnimalOnColorChange(string animalId, string jsonData)
        {
            // 딕셔너리 데이터 교체 후 썸네일 다시 만들어주는 것까지 해야 함
            animalDataArray = JsonHelper.FromJson<AnimalDataFormat>(jsonData);
            AnimalDataFormat updatedAnimalData;
            // 업데이트 된 동물의 정보 찾기
            for(int i = 0; i < animalDataArray.Length; i++)
            {
                if(animalDataArray[i].id == animalId)
                {
                    updatedAnimalData = animalDataArray[i]; 

                    // 업데이트 할 동물의 오브젝트를 딕셔너리에서 가져옴
                    GameObject animalObj = animalObjectDictionary[updatedAnimalData.id];
                    Animal animal = new Animal(updatedAnimalData);
                    // animal의 텍스처 변경
                    animalFactory.ChangeTextureAnimalObject(animalObj, animal);

                    GameObject.FindObjectOfType<SynthesisManager>().RefreshAnimalThumbnail(animalObj, updatedAnimalData);
                }
            }
        }

        // 합성 이후 다시 데이터를 불러와야 함 
        public void RefreshAnimalDataFusion(string animalId1, string animalId2, string resultAnimalId)
        {
            StartCoroutine(UpdateDataOnFusion(ApiUrl.getUserAnimal, animalId1, animalId2, resultAnimalId));
        }

        private IEnumerator UpdateDataOnFusion(string URL, string animalId1, string animalId2, string resultAnimalId)
        {   
            UnityWebRequest request = UnityWebRequest.Get(URL);
            var access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            // TODO: 임시로 설정
            access_token = tempAccessToken;

            Debug.Log($"access token = {access_token}");
            request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string jsonData = request.downloadHandler.text;
                SetUpdatedAnimalOnFusion(animalId1, animalId2, resultAnimalId, jsonData);
            }
        }

        // 합성 이후 업데이트 된 동물의 딕셔너리 데이터를 추가해주자
        private void SetUpdatedAnimalOnFusion(string animalId1, string animalId2, string resultAnimalId, string jsonData)
        {
            // 딕셔너리 데이터 교체 후 썸네일 다시 만들어주는 것까지 해야 함
            animalDataArray = JsonHelper.FromJson<AnimalDataFormat>(jsonData);
            AnimalDataFormat updatedAnimalData;
            // 새로운 추가된 정보를 가져옴
            // 딕셔너리에서 재료로 쓰인 동물 삭제
            animalObjectDictionary.Remove(animalId1);
            animalObjectDictionary.Remove(animalId2);


            // 서버에서 새로 받아온 데이터에서 추가된 동물의 정보 찾기
            for(int i = 0; i < animalDataArray.Length; i++)
            {
                if(animalDataArray[i].id == resultAnimalId)
                {
                    updatedAnimalData = animalDataArray[i]; 

                    // 업데이트 할 동물의 오브젝트를 딕셔너리에서 가져옴
                    Animal animal = new Animal(updatedAnimalData);
                    // animalFactory를 통해 새로운 오브젝트 생성
                    GameObject animalObj = animalFactory.GetAnimalGameObject(animal);
                    // 딕셔너리에 추가
                    animalObjectDictionary.Add(updatedAnimalData.id, animalObj);
                    // synthesisManager 에서 썸네일 추가해줌 
                    GameObject.FindObjectOfType<SynthesisManager>().RefreshAnimalThumbnail(animalObj, updatedAnimalData);
                }
            }
        }

        public GameObject GetAnimalObject(string id)
        {
            GameObject obj = null;
            if (animalObjectDictionary.ContainsKey(id))
            {
                obj = animalObjectDictionary[id];
            }
            return obj;
        }
    }

}

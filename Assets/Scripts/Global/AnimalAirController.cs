using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BluehatGames
{

    // 서버에서 동물 정보를 받아옴
    public class AnimalAirController : MonoBehaviour
    {
        public AnimalFactory animalFactory;

        public string tempAccessToken = "0000";
        private List<GameObject> animalObjectList;
        private AnimalDataFormat[]  animalDataArray;

        private Scene currentScene;
        private string currentSceneName;

        void Start()
        {
            animalObjectList = new List<GameObject>();
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
                switch(currentSceneName)
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
            animalObjectList = animalFactory.ConvertJsonToAnimalObject(jsonData);

            // 메인 씬에 동물 배치
            for(int i = 0; i < animalObjectList.Count; i++)
            {
                GameObject animalObject = animalObjectList[i];
                float randomX = Random.Range(-20, 20);
                float randomZ = Random.Range(-20, 20);
                animalObject.transform.position = new Vector3(randomX, 0.1f, randomZ);
                animalObject.transform.rotation = Quaternion.identity;
                animalObject.AddComponent<MainSceneAnimal>();
            }
        }

        private void SetSynthesisSceneAnimals(string jsonData)
        {
            animalDataArray = JsonHelper.FromJson<AnimalDataFormat>(jsonData);
            for(int i = 0; i <  animalDataArray.Length; i++)
            {
                Debug.Log($"animal_id = {animalDataArray[i].id}, animal_type = {animalDataArray[i].animalType}");
            }

            GameObject.FindObjectOfType<SynthesisManager>().StartMakeThumbnailAnimalList(animalDataArray);
        }
    }

}

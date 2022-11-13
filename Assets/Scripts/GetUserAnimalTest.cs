using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    [System.Serializable]
    public class AnimalDataFromServer
    {
        public string name;
        public int tier;
        public string color;
        public string animalType;
        public string headItem;
        public string bodyItem;
        public string footItem;
        public string pattern;

    }
    public class GetUserAnimalTest : MonoBehaviour
    {
        void Start()
        {

            StartCoroutine(DownLoadGet(ApiUrl.getAnimalList));

        }

        public AnimalDataFromServer[] animalData;
        public IEnumerator DownLoadGet(string URL)
        {
            UnityWebRequest request = UnityWebRequest.Get(URL);
            request.SetRequestHeader("Authorization", AccessToken.GetAccessToken());
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                var animalData = JsonHelper.FromJson<AnimalDataFromServer>(request.downloadHandler.text);
                Debug.Log($"animalData = ${animalData.Length}");

                for (int i = 0; i < animalData.Length; i++)
                {
                    Debug.Log($"animalData[i].name = {animalData[i].name}, animalData[i].type = {animalData[i].animalType}");

                }
            }
        }

    }
}
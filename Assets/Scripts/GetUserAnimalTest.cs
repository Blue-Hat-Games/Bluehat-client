using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    [Serializable]
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
        public AnimalDataFromServer[] animalData;

        private void Start()
        {
            StartCoroutine(DownLoadGet(ApiUrl.getAnimalList));
        }

        public IEnumerator DownLoadGet(string URL)
        {
            var request = UnityWebRequest.Get(URL);
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

                for (var i = 0; i < animalData.Length; i++)
                    Debug.Log(
                        $"animalData[i].name = {animalData[i].name}, animalData[i].type = {animalData[i].animalType}");
            }
        }
    }
}
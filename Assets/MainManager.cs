using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    public class MainManager : MonoBehaviour
    {
        private string key_autoStatus = "AuthStatus";
        
        void Start()
        {
            if (GetClientInfo(key_autoStatus) == AuthStatus._JOIN_COMPLETED)
            {
                Debug.Log("첫 번째 동물 획득 플로우");
                StartCoroutine(GetFirstAnimalFromServer(ApiUrl.animalNew));
            }
        }
        void SaveClientInfo(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        int GetClientInfo(string key)
        {
            return PlayerPrefs.GetInt(key);
        }


        public IEnumerator GetFirstAnimalFromServer(string URL)
        {
              // 웹서버로 Post 요청을 보냄
            using (UnityWebRequest request = UnityWebRequest.Post(URL, ""))
            {
                // request.uploadHandler = new UploadHandlerRaw(byteEmail); // 업로드 핸들러
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // 다운로드 핸들러
                                                                                        // 헤더를 Json으로 설정
                // request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {                 
                    // 웹서버로부터 받은 응답 내용 출력
                    Debug.Log(request.downloadHandler.text);
                    var animalName = request.downloadHandler.text;
                    LoadAnimalPrefab(animalName);
                }
            }
        }

        private void LoadAnimalPrefab(string animalName) {
            var path = $"Assets/Prefab/Animals/{animalName}.prefab";
            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            Instantiate(obj, Vector3.zero, Quaternion.identity);
            Debug.Log($"Creating Animal is Success! => {animalName}");
        }

    }
}
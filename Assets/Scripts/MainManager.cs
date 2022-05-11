using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BluehatGames
{

    public class MainManager : MonoBehaviour
    {
        public Button btn_synthesis;
        public Button btn_multiplay;

        private DataManager dataManager;

        public Text text_fistAnimal;

        void Start()
        {
            text_fistAnimal.gameObject.SetActive(false);
            dataManager = GameObject.FindObjectOfType<DataManager>();

            if (GetClientInfo(PlayerPrefsKey.key_authStatus) == AuthStatus._JOIN_COMPLETED)
            {
                Debug.Log("첫 번째 동물 획득 플로우");
                StartCoroutine(GetFirstAnimalFromServer(ApiUrl.postAnimalNew));
            }

            btn_synthesis.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._04_Synthesis);
            });
            btn_multiplay.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._05_Multiplay);
            });
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

                //request.uploadHandler = new UploadHandlerRaw(byteInfo); // 업로드 핸들러
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // 다운로드 핸들러
                                                                                        // 헤더를 Json으로 설정

                string access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
                Debug.Log($"access_token = {access_token}");
                request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);

                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    // 웹서버로부터 받은 응답 내용 출력
                    string responseText = request.downloadHandler.text;
                    string responseType = JsonUtility.FromJson<ResponseAnimalNew>(responseText).type;

                    Debug.Log(request.downloadHandler.text);
                    var animalName = responseType;
                    dataManager.AddNewAnimal(animalName);
                    LoadAnimalPrefab(animalName);

                    text_fistAnimal.text = $"Your First Animal is {animalName}!";
                    text_fistAnimal.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2);
                    text_fistAnimal.gameObject.SetActive(false);
                }
            }
        }

        private void LoadAnimalPrefab(string animalName) {
            var path = $"Assets/Prefab/Animals/{animalName}.prefab";
            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            GameObject animal = Instantiate(obj, Vector3.zero, Quaternion.identity);
            animal.transform.LookAt(Camera.main.transform);
            Debug.Log($"Creating Animal is Success! => {animalName}");
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
namespace BluehatGames
{

    public class MainManager : MonoBehaviour
    {
        public Button btn_synthesis;
        public Button btn_multiplay;
        public Button btn_nftMarket;
        public Button btn_exit;

        private DataManager dataManager;

        public Text text_fistAnimal;

        void Start()
        {
            text_fistAnimal.gameObject.SetActive(false);
            dataManager = GameObject.FindObjectOfType<DataManager>();

            if (GetClientInfo(PlayerPrefsKey.key_authStatus) == AuthStatus._JOIN_COMPLETED)
            {
                Debug.Log("�? 번째 ?���? ?��?�� ?��로우");
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

            btn_nftMarket.onClick.AddListener(() => {
                // web ����
            });

            btn_exit.onClick.AddListener(() => {
                Application.Quit();
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

            // ?��?��버로 Post ?���??�� 보냄
            using (UnityWebRequest request = UnityWebRequest.Post(URL, ""))
            {

                //request.uploadHandler = new UploadHandlerRaw(byteInfo); // ?��로드 ?��?��?��
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // ?��?��로드 ?��?��?��
                                                                                        // ?��?���? Json?���? ?��?��

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
                    // ?��?��버로�??�� 받�?? ?��?�� ?��?�� 출력
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
            var path = $"Prefab/Animals/{animalName}";
            GameObject obj = Resources.Load(path) as GameObject;
            GameObject animal = Instantiate(obj, Vector3.zero, Quaternion.identity);
            animal.transform.LookAt(Camera.main.transform);
            Debug.Log($"Creating Animal is Success! => {animalName}");
        }

    }
}
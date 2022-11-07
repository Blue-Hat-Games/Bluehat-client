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
        public Button btn_nftMarket;
        // public Button btn_exit;

        [Header("Setting Button")]
        public Button btn_setting;
        public GameObject settingPanel;
        public Button btn_setting_close;
        public Button btn_logout;
        public Toggle toggle_music;
        public Toggle toggle_sound_effect;
        private DataManager dataManager;

        [Header("Alert Panel")]
        public Text text_fistAnimal;
        public Button AlertDoneBtn;
        public GameObject AlertPanel;

        [Header("Music")]
        public AudioSource audioSource;
        private SoundUtil soundUtil;

        void Start()
        {
            dataManager = GameObject.FindObjectOfType<DataManager>();
            AlertPanel.SetActive(false);
            soundUtil = new SoundUtil();

            if (soundUtil.isbackgroundMusicOn())
            {
                audioSource.Play();
                toggle_music.isOn = true;
            }
            else
            {
                audioSource.Stop();
                toggle_music.isOn = false;
            }

            if (GetClientInfo(PlayerPrefsKey.key_authStatus) == AuthStatus._JOIN_COMPLETED)
            {
                Debug.Log("Player Status => Join Completed");
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

            btn_nftMarket.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._06_Market);
            });


            settingPanel.SetActive(false);
            btn_logout.onClick.AddListener(() =>
            {
                AuthKey.ClearAuthKey();
                PlayerPrefs.SetInt(PlayerPrefsKey.key_authStatus, AuthStatus._INIT);
                SceneManager.LoadScene(SceneName._01_Title);
            });

            btn_setting.onClick.AddListener(() =>
            {
                settingPanel.SetActive(true);
            });

            btn_setting_close.onClick.AddListener(() =>
            {
                settingPanel.SetActive(false);
            });

            AlertDoneBtn.onClick.AddListener(() =>
            {
                AlertPanel.SetActive(false);
            });

            toggle_music.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                {
                    audioSource.Play();
                    soundUtil.turnOnBackgroundMusic();
                }
                else
                {
                    audioSource.Stop();
                    soundUtil.turnOffBackgroundMusic();
                }
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


            using (UnityWebRequest request = UnityWebRequest.Post(URL, ""))
            {

                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                // Access Token
                string access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
                Debug.Log($"access_token = {access_token}");
                // send access token to server
                request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);

                yield return request.SendWebRequest();

                // error
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                // success
                else
                {
                    string responseText = request.downloadHandler.text;
                    string responseType = JsonUtility.FromJson<ResponseAnimalNew>(responseText).type;

                    Debug.Log(request.downloadHandler.text);

                    var animalName = responseType;
                    // Data manager
                    dataManager.AddNewAnimal(animalName);
                    LoadAnimalPrefab(animalName);

                    AlertPanel.SetActive(true);
                    text_fistAnimal.text = $"Your First Animal is {animalName}!";
                    text_fistAnimal.gameObject.SetActive(true);
                    SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._GENERAL_USER);
                }
            }
        }

        private void LoadAnimalPrefab(string animalName)
        {
            var path = $"Prefab/Animals/{animalName}";
            GameObject obj = Resources.Load(path) as GameObject;
            GameObject animal = Instantiate(obj, Vector3.zero, Quaternion.identity);
            animal.transform.LookAt(Camera.main.transform);
            Debug.Log($"Creating Animal is Success! => {animalName}");
        }

    }
}
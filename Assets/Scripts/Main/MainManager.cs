using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BluehatGames
{
    public class MainManager : MonoBehaviour
    {
        public Button btn_synthesis;
        public Button btn_multiplay;

        public Button btn_nftMarket;
        // public Button btn_exit;

        [Header("Setting Button")] public Button btn_setting;

        public GameObject settingPanel;
        public Button btn_setting_close;
        public Button btn_logout;
        public Toggle toggle_music;
        public Toggle toggle_sound_effect;

        [Header("Alert Panel")] public Text text_fistAnimal;

        public Button AlertDoneBtn;
        public GameObject AlertPanel;

        [Header("Music")] public AudioSource audioSource;

        public AudioClip multiplayButtonSound;
        public AudioClip upperButtonSound;
        public AudioClip mainButtonSound;
        private DataManager dataManager;
        private SoundUtil soundUtil;

        private void Start()
        {
            dataManager = FindObjectOfType<DataManager>();
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
                SoundManager.instance.PlayEffectSound(mainButtonSound);
                SceneManager.LoadScene(SceneName._04_Synthesis);
            });

            btn_multiplay.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(multiplayButtonSound);
                SceneManager.LoadScene(SceneName._05_Multiplay);
            });

            btn_nftMarket.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(mainButtonSound);
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
                SoundManager.instance.PlayEffectSound(upperButtonSound);
                settingPanel.SetActive(true);
            });

            btn_setting_close.onClick.AddListener(() => { settingPanel.SetActive(false); });

            AlertDoneBtn.onClick.AddListener(() => { AlertPanel.SetActive(false); });

            toggle_music.onValueChanged.AddListener(value =>
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

        private void SaveClientInfo(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        private int GetClientInfo(string key)
        {
            return PlayerPrefs.GetInt(key);
        }


        public IEnumerator GetFirstAnimalFromServer(string URL)
        {
            using (var request = UnityWebRequest.Post(URL, ""))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                // Access Token
                var access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
                Debug.Log($"access_token = {access_token}");
                // send access token to server
                request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);

                yield return request.SendWebRequest();

                // error
                if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                // success
                else
                {
                    var responseText = request.downloadHandler.text;
                    var responseType = JsonUtility.FromJson<ResponseAnimalNew>(responseText).type;

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
            var obj = Resources.Load(path) as GameObject;
            var animal = Instantiate(obj, Vector3.zero, Quaternion.identity);
            animal.transform.LookAt(Camera.main.transform);
            Debug.Log($"Creating Animal is Success! => {animalName}");
        }
    }
}
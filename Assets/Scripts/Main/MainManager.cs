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
        public AudioClip multiplayButtonSound;
        public AudioClip upperButtonSound;
        public AudioClip mainButtonSound;

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
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt(PlayerPrefsKey.key_authStatus, AuthStatus._INIT);
                SceneManager.LoadScene(SceneName._01_Title);
            });

            btn_setting.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(upperButtonSound);
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

    }
}
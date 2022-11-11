using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames
{
    public class DebugManager : MonoBehaviour
    {
        public Button btn_status_init;
        public Button btn_status_email_authenticating;
        public Button btn_status_join_completed;
        public Button btn_status_general_user;
        public Toggle toggle_debugManager;

        public GameObject debugPanel;
        private readonly string key_autoStatus = "AuthStatus";

        private void Start()
        {
            debugPanel.SetActive(false);
            toggle_debugManager.onValueChanged.AddListener(onoff => { debugPanel.SetActive(onoff); });

            btn_status_init.onClick.AddListener(() =>
            {
                Debug.Log("[DebugManager] Current Status - INIT");
                SaveClientInfo(key_autoStatus, AuthStatus._INIT);
            });
            btn_status_email_authenticating.onClick.AddListener(() =>
            {
                Debug.Log("[DebugManager] Current Status - EMAIL_AUTHENTICATING");
                SaveClientInfo(key_autoStatus, AuthStatus._EMAIL_AUTHENTICATING);
            });
            btn_status_join_completed.onClick.AddListener(() =>
            {
                Debug.Log("[DebugManager] Current Status - JOIN_COMPLETED");
                SaveClientInfo(key_autoStatus, AuthStatus._JOIN_COMPLETED);
            });
            btn_status_general_user.onClick.AddListener(() =>
            {
                Debug.Log("[DebugManager] Current Status - GENERAL_USER");
                SaveClientInfo(key_autoStatus, AuthStatus._GENERAL_USER);
            });
        }

        private void SaveClientInfo(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
    }
}
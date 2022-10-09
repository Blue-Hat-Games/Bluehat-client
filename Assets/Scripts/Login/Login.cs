using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BluehatGames
{


    public class PlayerJoinInfo
    {
        public string email;
        public string wallet_address;
    }
    public class PlayerInfo
    {
        public string email;
        public string wallet_address;
    }


    public class Login : MonoBehaviour
    {
        [Header("Buttons")]
        public Button btn_login;
        public Button btn_refresh;
        public Button btn_play;

        [Header("InputFields")]
        public InputField inputEmail;
        public InputField inputWallet;
        public string URL;

        [Header("Alert Popup")]
        public GameObject alertPopup;
        public Text alertText;
        private string emailMessage = "Email OK.";
        private string authCompleted = "Auth OK";
        private string warnEmailMessage = "Email Not OK.";
        private string warnWalletMessage = "Wallet Address Not OK.";


        [Header("Control Variables")]
        public int popupShowTime;


        private Coroutine popupCoroutine;

        [Header("ForTest")]
        public bool isCompletedAuth;

        void SaveClientInfo(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        int GetClientInfo(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        void Start()
        {
            Debug.Log(Application.persistentDataPath);

            SaveData loadData = SaveSystem.LoadUserInfoFile();
            if (loadData != null)
            {
                Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
            }
            else
            {
                Debug.Log("loadData is null");
            }

            Debug.Log($"Client Current Status => {GetClientInfo(PlayerPrefsKey.key_authStatus)}");

            // login button onClick
            btn_login.onClick.AddListener(() =>
            {
                if (false == IsValidInputData(inputEmail.text, inputWallet.text))
                {
                    Debug.Log("Input Data is INVALIED");
                    return;
                }

                btn_refresh.gameObject.SetActive(true);
                StartCoroutine(RequestAuthToServer(ApiUrl.emailLoginVerify, inputEmail.text, inputWallet.text, (UnityWebRequest request) =>
                {
                    StartCoroutine(ShowAlertPopup(emailMessage));

                    // json text from server response
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response.msg => {response.msg}");

                    if (response.msg != "fail")
                    {
                        SaveData user = new SaveData(inputEmail.text, inputWallet.text);
                        SaveSystem.SaveUserInfoFile(user);
                        Debug.Log($"SaveSystem | Save User Info File ({inputEmail.text}, {inputWallet.text})");
                    }
                }));
            });

            // refresh button onClick
            btn_refresh.onClick.AddListener(() =>
            {
                SaveData loadData = SaveSystem.LoadUserInfoFile();
                if (loadData != null)
                {
                    Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
                }
                else
                {
                    Debug.Log("Save Data is null");
                    return;
                }


                StartCoroutine(RequestAuthToServer(ApiUrl.login, loadData.email, loadData.wallet_address, (UnityWebRequest request) =>
                {

                    Debug.Log($"request.downloadHandler.text = {request.downloadHandler.text}");
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response} | response.msg = {response.msg}");
                    if (response.msg == "Register Success" || response.msg == "Login Success")
                    {
                        if (null != popupCoroutine)
                        {
                            StopCoroutine(popupCoroutine);
                        }

                        StartCoroutine(ShowAlertPopup(authCompleted));

                        SetJoinCompletedSetting();
                        SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._JOIN_COMPLETED);
                        PlayerPrefs.SetString(PlayerPrefsKey.key_accessToken, response.access_token);
                        Debug.Log("Login access_token response => " + PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken));

                    }
                    else
                    {
                        Debug.LogError("Server: Email not Verified.");
                    }
                }));
            });



            // play button onClick
            btn_play.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });


            // hide play button, refresh button in first
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);

            // get current client auth info 
            var clientAuthInfo = GetClientInfo(PlayerPrefsKey.key_authStatus);

            // email authenticating
            if (clientAuthInfo == AuthStatus._EMAIL_AUTHENTICATING)
            {
                SetEmailAuthenticatingSetting();
            }
            else if (clientAuthInfo == AuthStatus._JOIN_COMPLETED)
            {

                SetJoinCompletedSetting();
            }
            else
            {
                SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._INIT);
            }
        }

        public void SetEmailAuthenticatingSetting()
        {
            btn_login.GetComponentInChildren<Text>().text = "Resend\n Email";
            btn_refresh.gameObject.SetActive(true);
        }

        public void SetJoinCompletedSetting()
        {
            btn_play.gameObject.SetActive(true);
        }


        IEnumerator ShowAlertPopup(string text)
        {
            alertText.text = text;
            alertPopup.SetActive(true);
            yield return new WaitForSeconds(popupShowTime);
            alertPopup.SetActive(false);
        }

        IEnumerator RequestAuthToServer(string URL, string inputEmail, string inputWallet, Action<UnityWebRequest> action)
        {
            Debug.Log($"RequestAuthToServer | URL: {URL}, inputEmail: {inputEmail}, inputWallet: {inputWallet}");
            string jsonData = "";

            // 'emailLoginVerify' or 'login'
            if (URL == ApiUrl.emailLoginVerify)
            {
                jsonData = SetPlayerJoinInfoToJsonData(inputEmail, inputWallet);

                if (null != popupCoroutine)
                {
                    StopCoroutine(popupCoroutine);
                }

                // alert popup 
                StartCoroutine(ShowAlertPopup(emailMessage));
            }
            else if (URL == ApiUrl.login)
            {
                // inputEmail, inputWallet to jsonData
                jsonData = SetPlayerInfoToJsonData(inputEmail, inputWallet);

            }
            Debug.Log(jsonData);


            btn_login.GetComponentInChildren<Text>().text = "Resend Email";

            // byteEmail 
            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("request Error!");
                    Debug.Log(request.error + " | " + request);
                }
                else
                {
                    Debug.Log("request Success! Action Invoke");
                    action.Invoke(request);

                    // URL -> 'emailLoginVerify' or 'login'
                    if (URL == ApiUrl.emailLoginVerify)
                    {
                        SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._EMAIL_AUTHENTICATING);
                    }
                    else
                    {
                        SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._JOIN_COMPLETED);
                    }
                }
            }
        }


        string SetPlayerJoinInfoToJsonData(string inputEmail, string inputWallet)
        {
            PlayerJoinInfo playerInfo = new PlayerJoinInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo);

        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {

            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo); ;

        }

        bool IsValidInputData(string inputEmail, string inputWallet)
        {
            // warn email
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
                return false;
            }
            // input wallet 
            if ("" == inputWallet)
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnWalletMessage));
                return false;
            }

            return true;
        }

        // check valid email
        private bool IsValidEmail(string email)
        {
            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            return valid;
        }
    }
}
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
        private string emailMessage = "?΄λ©μΌ?? ?Έμ¦μ ?λ£ν΄μ£ΌμΈ?.";
        private string authCompleted = "λ‘κ·Έ?Έ? ?±κ³΅ν?΅??€!";
        private string warnEmailMessage = "?¬λ°λ₯Έ ?΄λ©μΌ μ£Όμκ°? ????€.";
        private string warnWalletMessage = "?¬λ°λ₯Έ μ§?κ°? μ£Όμκ°? ????€.";


        [Header("Control Variables")]
        public int popupShowTime;


        private Coroutine popupCoroutine;

        // PlayerPref? ????₯?  κ²?
        // 1. ?΄λ©μΌ ?Έμ¦μ λ³΄λ΄? ?λ£λ§ ?λ©? ??μ§?
        // - Login λ²νΌ? ?΄λ©μΌ ?€? λ³΄λ΄κΈ? λ²νΌ?Όλ‘? λ³?κ²? 
        // 2. ?΄λ©μΌ ?Έμ¦μ ?λ£ν?μ§?
        // - ? ??κΈ? λ²νΌ?Όλ‘? λ³?κ²?

        [Header("ForTest")]
        public bool isCompletedAuth;

        void SaveClientInfo(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        int GetClientInfo(string key) {
            return PlayerPrefs.GetInt(key);
        }

        void Start()
        {
            SaveData loadData = SaveSystem.LoadUserInfoFile();
            if (loadData != null)
            {
                Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
            }

            Debug.Log($"Client Current Status => {GetClientInfo(PlayerPrefsKey.key_authStatus)}");
            // λ‘κ·Έ?Έ λ²νΌ onClick
            btn_login.onClick.AddListener(() =>
            {
                
                if (false == IsValidInputData(inputEmail.text, inputWallet.text))
                    return;

                btn_refresh.gameObject.SetActive(true);
                StartCoroutine(RequestAuthToServer(ApiUrl.emailLoginVerify, inputEmail.text, inputWallet.text, (UnityWebRequest request) =>
                {
                    StartCoroutine(ShowAlertPopup(emailMessage));

                    // json text from server response
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response.msg}");

                    if (response.msg != "fail")
                    {
                        SaveData user = new SaveData(inputEmail.text, inputWallet.text);
                        SaveSystem.SaveUserInfoFile(user);
                    }
                }));
            });

            // λ¦¬ν? ? λ²νΌ onClick 
            btn_refresh.onClick.AddListener(() =>
            {
                SaveData loadData = SaveSystem.LoadUserInfoFile();
                if(loadData != null)
                {
                    Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
                }

                // ?΄ ? ???κ°? ?Έμ¦? ?λ£λ ? ????Έμ§? ?λ²μ ??Έ ?μ²??¨ 
                // - ?Έμ¦? ?λ£λ ? ????΄λ©? ? ??κΈ? λ²νΌ ??±?
                StartCoroutine(RequestAuthToServer(ApiUrl.login, loadData.email, loadData.wallet_address, (UnityWebRequest request) =>
                {                  
                    // ?Ή?λ²λ‘λΆ??° λ°μ?? ??΅ ?΄?© μΆλ ₯
                    Debug.Log(request.downloadHandler.text);
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response} | response.msg = {response.msg}");
                    if(response.msg == "Register Success" || response.msg == "Login Success")
                    {
                        if (null != popupCoroutine)
                        {
                            // κΈ°μ‘΄ μ½λ£¨?΄?΄ ???€λ©? ? μ§???€κ³? ?λ‘μ΄ μ½λ£¨?΄?΄ ?€???λ‘? ?¨ 
                            StopCoroutine(popupCoroutine);
                        }

                        StartCoroutine(ShowAlertPopup(authCompleted));

                        SetJoinCompletedSetting();
                        SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._JOIN_COMPLETED);
                        PlayerPrefs.SetString(PlayerPrefsKey.key_accessToken, response.access_token);
                        Debug.Log("Login access_token response => " + PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken));
                        
                    } else
                    {
                        Debug.LogError("Server: Email not Verified.");
                    }
                }));
            });



            // ? ??κΈ? λ²νΌ
            // 1. κΈ°λ³Έ??? λΉν?±?
            // 2. ??±? ?? ??  ?
            // - Refresh λ²νΌ? ??¬? ?΄λ©μΌ ?Έμ¦λ κ²? ??Έ??? κ²½μ°
            // - 
            btn_play.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });


            // μ²μ?? ?? ?΄ λ²νΌ, λ¦¬ν? ? λ²νΌ λΉν?±?
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);


            var clientAuthInfo = GetClientInfo(PlayerPrefsKey.key_authStatus);
            // ?Έμ¦? ??? ?°?Ό λΆκΈ° 
            // ?΄λ©μΌ? λ³΄λΈ ???΄λ©? ?΄λ©μΌ ?¬? ?‘ λ²νΌ ??
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
                // ?λ¬΄λ° ? λ³΄λ ??Όλ©? μ΄κΈ°?
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
        private bool IsValidEmail(string email)
        {
            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            return valid;
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
            Debug.Log($"RequestAuthToServer -> URL: {URL}");
            string jsonData = "";
            // ?΄λ©μΌκ³? μ§?κ°μ£Ό?λ₯? Json ???Όλ‘? λ³?? 
            if (URL == ApiUrl.emailLoginVerify)
            {
                jsonData = SetPlayerJoinInfoToJsonData(inputEmail, inputWallet);
                // TEST; ?΄λ©μΌ ?Έμ¦? ?λ£λ κ²½μ°? ?λ‘μ°
                if (null != popupCoroutine)
                {
                    // κΈ°μ‘΄ μ½λ£¨?΄?΄ ???€λ©? ? μ§???€κ³? ?λ‘μ΄ μ½λ£¨?΄?΄ ?€???λ‘? ?¨ 
                    StopCoroutine(popupCoroutine);
                }

                StartCoroutine(ShowAlertPopup(emailMessage));
            } 
            else if (URL == ApiUrl.login)
            {
                jsonData = SetPlayerInfoToJsonData(inputEmail, inputWallet);

            }
            Debug.Log(jsonData);


            btn_login.GetComponentInChildren<Text>().text = "Resend Email";

            // ?΄λ©μΌ? λ³΄λ?€? κ±? ????₯?¨
            SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._EMAIL_AUTHENTICATING);            

            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);
            // ?Ή?λ²λ‘ Post ?μ²?? λ³΄λ
            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail); // ?λ‘λ ?Έ?€?¬
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // ?€?΄λ‘λ ?Έ?€?¬
                                                                                        // ?€?λ₯? Json?Όλ‘? ?€? 
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
                }
            }
        }


        string SetPlayerJoinInfoToJsonData(string inputEmail, string inputWallet)
        {
            // ?λ²λ‘ λ³΄λΌ Json ?°?΄?° ??
            PlayerJoinInfo playerInfo = new PlayerJoinInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo);

        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {
            // ?λ²λ‘ λ³΄λΌ Json ?°?΄?° ??
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo); ;

        }

        bool IsValidInputData(string inputEmail, string inputWallet)
        {
            // 1. ? ?¨ ?°?΄?°?Έμ§? κ²??¬ 
            // - ?΄λ©μΌ μ£Όμκ°? ?λ§μ?? ???Έκ°?
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
                return false;
            }
            // - μ§?κ°? μ£Όμκ°? λΉμ΄??κ°?
            if ("" == inputWallet)
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnWalletMessage));
                return false;
            }

            return true;
        }
    }
}
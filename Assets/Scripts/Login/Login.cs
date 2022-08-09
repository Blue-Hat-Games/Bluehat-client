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

        // PlayerPref?— ????¥?•  ê²?
        // 1. ?´ë©”ì¼ ?¸ì¦ì„ ë³´ë‚´?„œ ?™„ë£Œë§Œ ?•˜ë©? ?˜?Š”ì§?
        // - Login ë²„íŠ¼?„ ?´ë©”ì¼ ?‹¤?‹œ ë³´ë‚´ê¸? ë²„íŠ¼?œ¼ë¡? ë³?ê²? 
        // 2. ?´ë©”ì¼ ?¸ì¦ì„ ?™„ë£Œí–ˆ?Š”ì§?
        // - ? ‘?†?•˜ê¸? ë²„íŠ¼?œ¼ë¡? ë³?ê²?

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
            // Áö±İ ¾î¶² °èÁ¤À¸·Î µÇ¾î ÀÖ´ÂÁö µğ¹ö±ë 
            SaveData loadData = SaveSystem.LoadUserInfoFile();
            if (loadData != null)
            {
                Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
            }

            Debug.Log($"Client Current Status => {GetClientInfo(PlayerPrefsKey.key_authStatus)}");
            // ë¡œê·¸?¸ ë²„íŠ¼ onClick
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

            // ë¦¬í”„? ˆ?‹œ ë²„íŠ¼ onClick 
            btn_refresh.onClick.AddListener(() =>
            {
                SaveData loadData = SaveSystem.LoadUserInfoFile();
                if(loadData != null)
                {
                    Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
                }

                // ÀÌ¸ŞÀÏ ÀÎÁõÇÒ ¶§ ÀÔ·ÂÇÑ °ªÀ» ·ÎÄÃ¿¡ ÀúÀåµÈ °É ºÒ·¯¿Í¼­ ÀÌ¸ŞÀÏ ÀÎÁõ ¿Ï·á ¿©ºÎ¿¡ ´ëÇØ ¼­¹ö¿¡ Ã¼Å©ÇÔ
                StartCoroutine(RequestAuthToServer(ApiUrl.login, loadData.email, loadData.wallet_address, (UnityWebRequest request) =>
                {                  
                    // ?›¹?„œë²„ë¡œë¶??„° ë°›ì?? ?‘?‹µ ?‚´?š© ì¶œë ¥
                    Debug.Log(request.downloadHandler.text);
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response} | response.msg = {response.msg}");
                    if(response.msg == "Register Success" || response.msg == "Login Success")
                    {
                        if (null != popupCoroutine)
                        {
                            // ê¸°ì¡´ ì½”ë£¨?‹´?´ ?ˆ?—ˆ?‹¤ë©? ? •ì§??‹œ?‚¤ê³? ?ƒˆë¡œìš´ ì½”ë£¨?‹´?´ ?‹¤?–‰?˜?„ë¡? ?•¨ 
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



            // ? ‘?†?•˜ê¸? ë²„íŠ¼
            // 1. ê¸°ë³¸??? ë¹„í™œ?„±?™”
            // 2. ?™œ?„±?™” ?˜?Š” ?‹œ?  ?
            // - Refresh ë²„íŠ¼?„ ?ˆŒ?Ÿ¬?„œ ?´ë©”ì¼ ?¸ì¦ëœ ê²? ?™•?¸?˜?—ˆ?„ ê²½ìš°
            // - 
            btn_play.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });


            // ì²˜ìŒ?—?Š” ?”Œ? ˆ?´ ë²„íŠ¼, ë¦¬í”„? ˆ?‹œ ë²„íŠ¼ ë¹„í™œ?„±?™”
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);


            var clientAuthInfo = GetClientInfo(PlayerPrefsKey.key_authStatus);
            // ?¸ì¦? ?ƒ?ƒœ?— ?”°?¼ ë¶„ê¸° 
            // ?´ë©”ì¼?„ ë³´ë‚¸ ?ƒ?ƒœ?´ë©? ?´ë©”ì¼ ?¬? „?†¡ ë²„íŠ¼ ?…‹?Œ…
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
                // ?•„ë¬´ëŸ° ? •ë³´ë„ ?—†?œ¼ë©? ì´ˆê¸°?™”
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
            Debug.Log($"RequestAuthToServer -> URL: {URL}");
            string jsonData = "";
            // ?´ë©”ì¼ê³? ì§?ê°‘ì£¼?†Œë¥? Json ?˜•?‹?œ¼ë¡? ë³??™˜ 
            if (URL == ApiUrl.emailLoginVerify)
            {
                jsonData = SetPlayerJoinInfoToJsonData(inputEmail, inputWallet);
                // TEST; ?´ë©”ì¼ ?¸ì¦? ?™„ë£Œëœ ê²½ìš°?˜ ?”Œë¡œìš°
                if (null != popupCoroutine)
                {
                    // ê¸°ì¡´ ì½”ë£¨?‹´?´ ?ˆ?—ˆ?‹¤ë©? ? •ì§??‹œ?‚¤ê³? ?ƒˆë¡œìš´ ì½”ë£¨?‹´?´ ?‹¤?–‰?˜?„ë¡? ?•¨ 
                    StopCoroutine(popupCoroutine);
                }

                StartCoroutine(ShowAlertPopup(emailMessage));
            } 
            else if (URL == ApiUrl.login)
            {
                // ÀÎÁõ Á¤º¸¸¦ ¿äÃ»ÇÏ·Á´Â µ¥ÀÌÅÍ¸¦ JsonÀ¸·Î ¹Ù²Ş 
                jsonData = SetPlayerInfoToJsonData(inputEmail, inputWallet);

            }
            Debug.Log(jsonData);


            btn_login.GetComponentInChildren<Text>().text = "Resend Email";

            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);
            // ?›¹?„œë²„ë¡œ Post ?š”ì²??„ ë³´ëƒ„
            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail); // ?—…ë¡œë“œ ?•¸?“¤?Ÿ¬
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // ?‹¤?š´ë¡œë“œ ?•¸?“¤?Ÿ¬
                                                                                        // ?—¤?”ë¥? Json?œ¼ë¡? ?„¤? •
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
                    // ÀÌ¸ŞÀÏ Àü¼Û¿¡ ¼º°øÇÑ °æ¿ì¿¡¸¸ ÀÎÁõ»óÅÂ¸¦ º¯°æ
                    if(URL == ApiUrl.emailLoginVerify)
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
            // ?„œë²„ë¡œ ë³´ë‚¼ Json ?°?´?„° ?…‹?Œ…
            PlayerJoinInfo playerInfo = new PlayerJoinInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo);

        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {
            // ?„œë²„ë¡œ ë³´ë‚¼ Json ?°?´?„° ?…‹?Œ…
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo); ;

        }

        bool IsValidInputData(string inputEmail, string inputWallet)
        {
            // 1. ?œ ?š¨ ?°?´?„°?¸ì§? ê²??‚¬ 
            // - ?´ë©”ì¼ ì£¼ì†Œê°? ?•Œë§ì?? ?˜•?‹?¸ê°?
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
                return false;
            }
            // - ì§?ê°? ì£¼ì†Œê°? ë¹„ì–´?ˆ?Š”ê°?
            if ("" == inputWallet)
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnWalletMessage));
                return false;
            }

            return true;
        }
        private bool IsValidEmail(string email)
        {
            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            return valid;
        }
    }
}
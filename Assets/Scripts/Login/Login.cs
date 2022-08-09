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

        // PlayerPref?�� ????��?�� �?
        // 1. ?��메일 ?��증을 보내?�� ?��료만 ?���? ?��?���?
        // - Login 버튼?�� ?��메일 ?��?�� 보내�? 버튼?���? �?�? 
        // 2. ?��메일 ?��증을 ?��료했?���?
        // - ?��?��?���? 버튼?���? �?�?

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
            // ���� � �������� �Ǿ� �ִ��� ����� 
            SaveData loadData = SaveSystem.LoadUserInfoFile();
            if (loadData != null)
            {
                Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
            }

            Debug.Log($"Client Current Status => {GetClientInfo(PlayerPrefsKey.key_authStatus)}");
            // 로그?�� 버튼 onClick
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

            // 리프?��?�� 버튼 onClick 
            btn_refresh.onClick.AddListener(() =>
            {
                SaveData loadData = SaveSystem.LoadUserInfoFile();
                if(loadData != null)
                {
                    Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
                }

                // �̸��� ������ �� �Է��� ���� ���ÿ� ����� �� �ҷ��ͼ� �̸��� ���� �Ϸ� ���ο� ���� ������ üũ��
                StartCoroutine(RequestAuthToServer(ApiUrl.login, loadData.email, loadData.wallet_address, (UnityWebRequest request) =>
                {                  
                    // ?��?��버로�??�� 받�?? ?��?�� ?��?�� 출력
                    Debug.Log(request.downloadHandler.text);
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response} | response.msg = {response.msg}");
                    if(response.msg == "Register Success" || response.msg == "Login Success")
                    {
                        if (null != popupCoroutine)
                        {
                            // 기존 코루?��?�� ?��?��?���? ?���??��?���? ?��로운 코루?��?�� ?��?��?��?���? ?�� 
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



            // ?��?��?���? 버튼
            // 1. 기본??? 비활?��?��
            // 2. ?��?��?�� ?��?�� ?��?�� ?
            // - Refresh 버튼?�� ?��?��?�� ?��메일 ?��증된 �? ?��?��?��?��?�� 경우
            // - 
            btn_play.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });


            // 처음?��?�� ?��?��?�� 버튼, 리프?��?�� 버튼 비활?��?��
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);


            var clientAuthInfo = GetClientInfo(PlayerPrefsKey.key_authStatus);
            // ?���? ?��?��?�� ?��?�� 분기 
            // ?��메일?�� 보낸 ?��?��?���? ?��메일 ?��?��?�� 버튼 ?��?��
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
                // ?��무런 ?��보도 ?��?���? 초기?��
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
            // ?��메일�? �?갑주?���? Json ?��?��?���? �??�� 
            if (URL == ApiUrl.emailLoginVerify)
            {
                jsonData = SetPlayerJoinInfoToJsonData(inputEmail, inputWallet);
                // TEST; ?��메일 ?���? ?��료된 경우?�� ?��로우
                if (null != popupCoroutine)
                {
                    // 기존 코루?��?�� ?��?��?���? ?���??��?���? ?��로운 코루?��?�� ?��?��?��?���? ?�� 
                    StopCoroutine(popupCoroutine);
                }

                StartCoroutine(ShowAlertPopup(emailMessage));
            } 
            else if (URL == ApiUrl.login)
            {
                // ���� ������ ��û�Ϸ��� �����͸� Json���� �ٲ� 
                jsonData = SetPlayerInfoToJsonData(inputEmail, inputWallet);

            }
            Debug.Log(jsonData);


            btn_login.GetComponentInChildren<Text>().text = "Resend Email";

            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);
            // ?��?��버로 Post ?���??�� 보냄
            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail); // ?��로드 ?��?��?��
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // ?��?��로드 ?��?��?��
                                                                                        // ?��?���? Json?���? ?��?��
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
                    // �̸��� ���ۿ� ������ ��쿡�� �������¸� ����
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
            // ?��버로 보낼 Json ?��?��?�� ?��?��
            PlayerJoinInfo playerInfo = new PlayerJoinInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo);

        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {
            // ?��버로 보낼 Json ?��?��?�� ?��?��
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo); ;

        }

        bool IsValidInputData(string inputEmail, string inputWallet)
        {
            // 1. ?��?�� ?��?��?��?���? �??�� 
            // - ?��메일 주소�? ?��맞�?? ?��?��?���?
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
                return false;
            }
            // - �?�? 주소�? 비어?��?���?
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
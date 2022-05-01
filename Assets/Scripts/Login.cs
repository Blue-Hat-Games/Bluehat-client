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
        private string emailMessage = "이메일에서 인증을 완료해주세요.";
        private string authCompleted = "로그인에 성공했습니다!";
        private string warnEmailMessage = "올바른 이메일 주소가 아닙니다.";
        private string warnWalletMessage = "올바른 지갑 주소가 아닙니다.";


        [Header("Control Variables")]
        public int popupShowTime;


        private Coroutine popupCoroutine;
        // PlayerPref
        private string key_authStatus = "AuthStatus";

        // PlayerPref에 저장할 것
        // 1. 이메일 인증을 보내서 완료만 하면 되는지
        // - Login 버튼을 이메일 다시 보내기 버튼으로 변경 
        // 2. 이메일 인증을 완료했는지
        // - 접속하기 버튼으로 변경

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
            Debug.Log($"Client Current Status => {GetClientInfo(key_authStatus)}");
            // 로그인 버튼 onClick
            btn_login.onClick.AddListener(() =>
            {
                
                if (false == IsValidInputData(inputEmail.text, inputWallet.text))
                    return;
                btn_refresh.gameObject.SetActive(true);
                StartCoroutine(RequestAuthToServer(ApiUrl.emailLoginVerify, inputEmail.text, inputWallet.text, (UnityWebRequest request) =>
                {
                    StartCoroutine(ShowAlertPopup(emailMessage));
                    // 웹서버로부터 받은 응답 내용 출력
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response.msg}");

                    if (response.msg != "fail")
                    {
                        SaveData user = new SaveData(inputEmail.text, inputWallet.text);
                        SaveSystem.Save(user, "userInfo");
                    }
                }));
            });

            // 리프레시 버튼 onClick 
            btn_refresh.onClick.AddListener(() =>
            {
                SaveData loadData = SaveSystem.Load("userInfo");
                if(loadData != null)
                {
                    Debug.Log($"Load Success! -> Email: {loadData.email} | walletAdd: {loadData.wallet_address}");
                }

                // 이 유저가 인증 완료된 유저인지 서버에 확인 요청함 
                // - 인증 완료된 유저이면 접속하기 버튼 활성화
                StartCoroutine(RequestAuthToServer(ApiUrl.login, loadData.email, loadData.wallet_address, (UnityWebRequest request) =>
                {                  
                    // 웹서버로부터 받은 응답 내용 출력
                    Debug.Log(request.downloadHandler.text);
                    var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                    Debug.Log($"response => {response} | response.msg = {response.msg}");
                    if(response.msg == "Register Success")
                    {
                        if (null != popupCoroutine)
                        {
                            // 기존 코루틴이 있었다면 정지시키고 새로운 코루틴이 실행되도록 함 
                            StopCoroutine(popupCoroutine);
                        }

                        StartCoroutine(ShowAlertPopup(authCompleted));

                        SetJoinCompletedSetting();
                        SaveClientInfo(key_authStatus, AuthStatus._JOIN_COMPLETED);
                        PlayerPrefs.SetString("access_token", response.access_token);
                        Debug.Log("Login access_token response => " + PlayerPrefs.GetString("access_token"));
                        
                    } else
                    {
                        Debug.LogError("Server: Email not Verified.");
                    }
                }));
            });



            // 접속하기 버튼
            // 1. 기본은 비활성화
            // 2. 활성화 되는 시점 ?
            // - Refresh 버튼을 눌러서 이메일 인증된 게 확인되었을 경우
            // - 
            btn_play.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });


            // 처음에는 플레이 버튼, 리프레시 버튼 비활성화
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);


            var clientAuthInfo = GetClientInfo(key_authStatus);
            // 인증 상태에 따라 분기 
            // 이메일을 보낸 상태이면 이메일 재전송 버튼 셋팅
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
                // 아무런 정보도 없으면 초기화
                SaveClientInfo(key_authStatus, AuthStatus._INIT);
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
            // 이메일과 지갑주소를 Json 형식으로 변환 
            if (URL == ApiUrl.emailLoginVerify)
            {
                jsonData = SetPlayerJoinInfoToJsonData(inputEmail, inputWallet);
                // TEST; 이메일 인증 완료된 경우의 플로우
                if (null != popupCoroutine)
                {
                    // 기존 코루틴이 있었다면 정지시키고 새로운 코루틴이 실행되도록 함 
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

            // 이메일을 보냈다는 걸 저장함
            SaveClientInfo(key_authStatus, AuthStatus._EMAIL_AUTHENTICATING);            

            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);
            // 웹서버로 Post 요청을 보냄
            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail); // 업로드 핸들러
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // 다운로드 핸들러
                                                                                        // 헤더를 Json으로 설정
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
            // 서버로 보낼 Json 데이터 셋팅
            PlayerJoinInfo playerInfo = new PlayerJoinInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo);

        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {
            // 서버로 보낼 Json 데이터 셋팅
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet_address = inputWallet;

            return JsonUtility.ToJson(playerInfo); ;

        }

        bool IsValidInputData(string inputEmail, string inputWallet)
        {
            // 1. 유효 데이터인지 검사 
            // - 이메일 주소가 알맞은 형식인가
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
                return false;
            }
            // - 지갑 주소가 비어있는가
            if ("" == inputWallet)
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnWalletMessage));
                return false;
            }

            return true;
        }
    }
}
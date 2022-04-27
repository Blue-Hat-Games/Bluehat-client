using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BluehatGames
{


    public class PlayerInfo
    {
        public string email;
        public string wallet;
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
        private string warnEmailMessage = "올바른 이메일 주소가 아닙니다.";
        private string warnWalletMessage = "올바른 지갑 주소가 아닙니다.";


        [Header("Control Variables")]
        public int popupShowTime;


        private Coroutine popupCoroutine;
        // PlayerPref
        private string key_autoStatus = "AuthStatus";

        private string url_emailLoginVerify = "http://api.bluehat.games/auth";
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
            // 로그인 버튼 onClick
            btn_login.onClick.AddListener(() =>
            {
                if (false == IsValidInputData(inputEmail.text, inputWallet.text))
                    return;
                
                StartCoroutine(RequestAuthToServer(ApiUrl.emailLoginVerify, inputEmail.text, inputWallet.text));
            });

            // 리프레시 버튼 onClick 
            btn_refresh.onClick.AddListener(() =>
            {
            // 이 유저가 인증 완료된 유저인지 서버에 확인 요청함 
            // - 인증 완료된 유저이면 접속하기 버튼 활성화
                if (isCompletedAuth)// Test
                {
                    btn_play.gameObject.SetActive(true);
                    SaveClientInfo(key_autoStatus, AuthStatus._JOIN_COMPLETED);
                }
            });



            // 접속하기 버튼
            // 1. 기본은 비활성화
            // 2. 활성화 되는 시점 ?
            // - Refresh 버튼을 눌러서 이메일 인증된 게 확인되었을 경우
            // - 
            btn_play.onClick.AddListener(() =>
            {

            });

            
            var clientAuthInfo = GetClientInfo(key_autoStatus);
            // 인증 상태에 따라 분기 
            // 이메일을 보낸 상태이면 이메일 재전송 버튼 셋팅
            if (clientAuthInfo == AuthStatus._EMAIL_AUTHENTICATING)
            {
                btn_login.GetComponentInChildren<Text>().text = "Resend\n Email";
            }
            else if(clientAuthInfo == AuthStatus._JOIN_COMPLETED) {
                
                btn_play.gameObject.SetActive(true);
            }
            else
            {
                // 아무런 정보도 없으면 초기화
                SaveClientInfo(key_autoStatus, AuthStatus._INIT);                
            }

            // 처음에는 플레이 버튼, 리프레시 버튼 비활성화
            btn_play.gameObject.SetActive(false);
            btn_refresh.gameObject.SetActive(false);
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


        IEnumerator RequestAuthToServer(string URL, string inputEmail, string inputWallet)
        {
            // 이메일과 지갑주소를 Json 형식으로 변환 
            string jsonData = SetPlayerInfoToJsonData(inputEmail, inputWallet);
            Debug.Log(jsonData);


            // TEST; 이메일 인증 완료된 경우의 플로우
            if (null != popupCoroutine)
            {
                // 기존 코루틴이 있었다면 정지시키고 새로운 코루틴이 실행되도록 함 
                StopCoroutine(popupCoroutine);
            }
            StartCoroutine(ShowAlertPopup(emailMessage));
            btn_login.GetComponentInChildren<Text>().text = "Resend Email";

            // 이메일을 보냈다는 걸 저장함
            SaveClientInfo(key_autoStatus, AuthStatus._EMAIL_AUTHENTICATING);            

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
                    Debug.Log(request.error);
                }
                else
                {
                    if (request.downloadHandler.text == "success")
                    {

                    }
                    StartCoroutine(ShowAlertPopup(emailMessage));
                    // 웹서버로부터 받은 응답 내용 출력
                    Debug.Log(request.downloadHandler.text);
                }
            }
        }

        string SetPlayerInfoToJsonData(string inputEmail, string inputWallet)
        {
            // 서버로 보낼 Json 데이터 셋팅
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.email = inputEmail;
            playerInfo.wallet = inputWallet;

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
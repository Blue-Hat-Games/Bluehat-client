using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BluehatGames
{
    public class LoginBtn
    {
        private Button btn_login;
        private Button btn_resend_email;
        private LoginBtnStatus btn_status;


        public enum LoginBtnStatus
        {
            SendEmail,
            Login
        }

        public LoginBtn(Button btn_login, Button btn_resend_email)
        {
            this.btn_login = btn_login;
            this.btn_resend_email = btn_resend_email;
        }

        public LoginBtnStatus GetBtnStatus()
        {
            return btn_status;
        }
        public void SetBtnSendEmail()
        {
            btn_status = LoginBtnStatus.SendEmail;
            btn_login.GetComponentInChildren<Text>().text = "Send Email";
            btn_resend_email.gameObject.SetActive(false);
        }

        public void SetBtnLogin()
        {
            btn_status = LoginBtnStatus.Login;
            btn_login.GetComponentInChildren<Text>().text = "Login";
            btn_resend_email.gameObject.SetActive(true);
        }


    }

    public class PlayerInfo
    {
        public string email;
        public PlayerInfo(string email)
        {
            this.email = email;
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }


    public class Login : MonoBehaviour
    {
        [Header("Buttons")]
        public Button btn_login;

        public Button btn_resend_email;

        [Header("InputFields")]
        public InputField inputEmail;

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
            LoginBtn loginBtn = new LoginBtn(btn_login, btn_resend_email);
            loginBtn.SetBtnSendEmail();

            // If click resend email button, login btn status change
            btn_resend_email.onClick.AddListener(() =>
            {
                loginBtn.SetBtnSendEmail();
            });

            // Login Btn Click
            btn_login.onClick.AddListener(() =>
            {
                string email = inputEmail.text;
                if (false == IsValidInputData(email))
                {
                    return; // Input Data is not Valid
                }

                // If Login Btn Status Send email
                if (loginBtn.GetBtnStatus() == LoginBtn.LoginBtnStatus.SendEmail)
                {

                    StartCoroutine(RequestAuthToServer(ApiUrl.emailLoginVerify, email, (UnityWebRequest request) =>
                    {
                        StartCoroutine(ShowAlertPopup(emailMessage));

                        // json text from server response
                        var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);

                        if (response.msg == "fail")
                        {
                            StartCoroutine(ShowAlertPopup("Can't send email."));
                            return;
                        }
                        else
                        {
                            Debug.Log("Send Email Success");
                            loginBtn.SetBtnLogin();
                        }
                    }));
                }

                // If Login Btn status is Login
                else
                {
                    StartCoroutine(RequestAuthToServer(ApiUrl.login, email, (UnityWebRequest request) =>
                    {
                        var response = JsonUtility.FromJson<ResponseLogin>(request.downloadHandler.text);
                        Debug.Log($"response => {response} | response.msg = {response.msg}");
                        if (response.msg == "Register Success" || response.msg == "Login Success")
                        {
                            if (null != popupCoroutine)
                            {
                                StopCoroutine(popupCoroutine);
                            }

                            StartCoroutine(ShowAlertPopup(authCompleted));
                            if (response.msg == "Register Success")
                            {
                                SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._JOIN_COMPLETED);
                            }
                            else
                            {
                                SaveClientInfo(PlayerPrefsKey.key_authStatus, AuthStatus._LOGIN_COMPLETED);
                            }
                            AuthKey.SetAuthKey(response.access_token);
                            SceneManager.LoadScene(SceneName._03_Main);
                        }
                        else
                        {
                            Debug.LogError("Server: Email not Verified.");
                        }
                    }));
                }

            });

        }


        IEnumerator ShowAlertPopup(string text)
        {
            alertText.text = text;
            alertPopup.SetActive(true);
            yield return new WaitForSeconds(popupShowTime);
            alertPopup.SetActive(false);
        }

        IEnumerator RequestAuthToServer(string URL, string inputEmail, Action<UnityWebRequest> action)
        {
            Debug.Log($"RequestAuthToServer | URL: {URL}, inputEmail: {inputEmail}");
            PlayerInfo playerInfo = new PlayerInfo(inputEmail);
            string jsonData = playerInfo.ToJson();
            Debug.Log("Resutlt = " + playerInfo.ToJson());

            // 'emailLoginVerify' or 'login'
            if (URL == ApiUrl.emailLoginVerify)
            {

                if (null != popupCoroutine)
                {
                    StopCoroutine(popupCoroutine);
                }

                // alert popup 
                StartCoroutine(ShowAlertPopup(emailMessage));
            }

            // byteEmail 
            byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
            {
                request.uploadHandler = new UploadHandlerRaw(byteEmail);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                if (request.responseCode == 409)
                {
                    Debug.Log("Email Not Verified");
                    StartCoroutine(ShowAlertPopup("Email Not Verified"));
                }
                else if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("request Error!");
                    Debug.Log(request.responseCode);
                    Debug.Log(request.error + " | " + request);
                }
                else
                {
                    Debug.Log("request Success!");
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


        bool IsValidInputData(string inputEmail)
        {
            // warn email
            if (false == IsValidEmail(inputEmail))
            {
                popupCoroutine = StartCoroutine(ShowAlertPopup(warnEmailMessage));
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
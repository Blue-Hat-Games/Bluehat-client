using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace BluehatGames
{
    public class TitleManager : MonoBehaviour
    {
        // Loading... 을 띄운다.
        // PlayerPref에서 "CompletedAuth" 가 true인 걸로 판단되면 메인 화면으로, 아니면 로그인 화면으로 보냄

        private string key_authStatus = "AuthStatus";
        public Text infoText;

        [Header("Scene name")]
        public string loginSceneName;
        public string mainSceneName;

        void Start()
        {
            StartCoroutine(ShowInfoText());
        }

        IEnumerator ShowInfoText()
        {
            while (true)
            {
                yield return null;
                yield return new WaitForSeconds(0.3f);
                infoText.text = "Loading.";
                yield return new WaitForSeconds(0.3f);
                infoText.text = "Loading..";
                yield return new WaitForSeconds(0.3f);
                infoText.text = "Loading...";

                if (PlayerPrefs.GetInt(key_authStatus) == AuthStatus._INIT || PlayerPrefs.GetInt(key_authStatus) == AuthStatus._EMAIL_AUTHENTICATING)
                {
                    infoText.text = "Please Login..";
                    yield return new WaitForSeconds(2);
                    SceneManager.LoadScene(SceneName._02_Login);
                }
                else
                {
                    if (AuthKey.CheckAuthKey())
                    {
                        infoText.text = "Login Success!";
                        yield return new WaitForSeconds(2);
                        SceneManager.LoadScene(SceneName._03_Main);

                    }
                    else
                    {
                        infoText.text = "Please Login..";
                        yield return new WaitForSeconds(2);
                        SceneManager.LoadScene(SceneName._02_Login);

                    }

                }
            }
        }

    }
}
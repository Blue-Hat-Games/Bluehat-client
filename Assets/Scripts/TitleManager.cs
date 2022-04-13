using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    // Loading... 을 띄운다.
    // PlayerPref에서 "CompletedAuth" 가 true인 걸로 판단되면 메인 화면으로, 아니면 로그인 화면으로 보냄
    private string key_sendedEmail = "SendedEmail";
    private string key_completedAuth = "CompletedAuth";
    public Text infoText;
    public Button btn_reset;

    [Header("Scene name")]
    public string loginSceneName;
    public string mainSceneName;
    
    void Start()
    {
        // 인증 정보 초기화 
        btn_reset.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(key_sendedEmail, 0);
            PlayerPrefs.SetInt(key_completedAuth, 0);
        });

        Debug.Log($"PlayerPrefs.GetInt(key_completedAuth) = {PlayerPrefs.GetInt(key_completedAuth)}");
        
        StartCoroutine(ShowInfoText());
    }

    
    void Update()
    {
        
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
            
            if(PlayerPrefs.GetInt(key_completedAuth) == 0)
            {
                infoText.text = "Please Login..";
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(loginSceneName);
            } 
            else
            {
                infoText.text = "Login Success!";
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(mainSceneName);
            }
        }
    }

}

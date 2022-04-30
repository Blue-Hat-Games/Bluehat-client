using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace BluehatGames
{
public class TitleManager : MonoBehaviour
{
    // Loading... �� ����.
    // PlayerPref���� "CompletedAuth" �� true�� �ɷ� �ǴܵǸ� ���� ȭ������, �ƴϸ� �α��� ȭ������ ����

    private string key_authStatus = "AuthStatus";
    public Text infoText;
    public Button btn_reset;

    [Header("Scene name")]
    public string loginSceneName;
    public string mainSceneName;
    
    void Start()
    {
        // ���� ���� �ʱ�ȭ 
        btn_reset.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(key_authStatus, AuthStatus._INIT);
            
        });

        Debug.Log($"PlayerPrefs.GetInt(key_completedAuth) = {PlayerPrefs.GetInt(key_authStatus)}");
        
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
            
            if(PlayerPrefs.GetInt(key_authStatus) == AuthStatus._INIT || PlayerPrefs.GetInt(key_authStatus) == AuthStatus._EMAIL_AUTHENTICATING)
            {
                infoText.text = "Please Login..";
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(SceneName._02_Login);
            } 
            else
            {
                infoText.text = "Login Success!";
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(SceneName._03_Main);
            }
        }
    }

}
}
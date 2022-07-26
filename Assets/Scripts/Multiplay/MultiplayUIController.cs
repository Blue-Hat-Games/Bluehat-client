using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace BluehatGames {

public class MultiplayUIController : MonoBehaviour
{
    public static MultiplayUIController instance = null;
    private void Awake()
    {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else if(instance != null) {
            Destroy(this.gameObject);
        }
    }

    // 멀티플레이에 이용되는 UI들 처리
    public GameObject startPanel;
    public Button goToMainButton;
    public Button startPanelExitButton;

    public Slider aetherProgressBar;
    public TextMeshProUGUI aetherCountText;

    public TextMeshProUGUI gameOverTime;

    public GameObject resultPanel;
    public TextMeshProUGUI resultObtainedAetherCoin;
    public TextMeshProUGUI resultMyAetherCoin;



     void Start()
    {
        aetherProgressBar.value = 0;
        resultPanel.SetActive(false);
        goToMainButton.onClick.AddListener(() => {
            MultiplayGameManager.instance.LeaveRoom();
            
        });

        startPanelExitButton.onClick.AddListener(() => {
            startPanel.SetActive(false);
        });
    }
    

    public void SetCurrentAetherCoinCount(int count)
    {
        aetherCountText.text = count.ToString();
    }
    
    public void ResetAetherProgressBar() {
        aetherProgressBar.value = 0;
    }

    public void SetAetherProgressBar(float value) {
        StartCoroutine(FadeSliderValue(value));
    }

    IEnumerator FadeSliderValue(float targetValue) {
        while(true) {
            yield return null;
            if(aetherProgressBar.value < targetValue) {
                aetherProgressBar.value += 0.01f;     
            } else {
                yield break;
            }
        }
    }

    public void UpdateGameTimeText(float gameTime)
    {
        if((int)gameTime >=10) {
            gameOverTime.text = $"00:{(int)gameTime}";
        } else {
            gameOverTime.text = $"00:0{(int)gameTime}";
        }
    }

    public void ResetGameTimeText()
    {
        gameOverTime.text = "00:00";
    }

    public void SetMultiplayResultPanel(int obtainedCoin, int myCoin) {
        resultObtainedAetherCoin.text = obtainedCoin.ToString();
        resultMyAetherCoin.text = myCoin.ToString();
        resultPanel.SetActive(true);
    }

}
}
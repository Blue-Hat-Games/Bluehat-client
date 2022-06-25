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
    public Slider aetherProgressBar;
    public GameObject resultPanel;
    public TextMeshProUGUI obtainedAetherCoin;
    public TextMeshProUGUI myAetherCoin;

    public Button goToMainButton;

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

    void Start()
    {
        aetherProgressBar.value = 0;
        resultPanel.SetActive(false);
        goToMainButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneName._03_Main);
        });
    }

    public void GameOver(int obtainedCoin, int myCoin) {
        obtainedAetherCoin.text = obtainedCoin.ToString();
        myAetherCoin.text = myCoin.ToString();
        resultPanel.SetActive(true);
    }

}
}
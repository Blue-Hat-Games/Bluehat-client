using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames
{
    public class MultiplayUIController : MonoBehaviour
    {
        public static MultiplayUIController instance;

        // 멀티플레이에 이용되는 UI들 처리
        public GameObject joystickCanvas;

        public GameObject startPanel;
        public Button goToMainButton;
        public Button startPanelExitButton;

        public Slider aetherProgressBar;
        public TextMeshProUGUI aetherCountText;

        public TextMeshProUGUI gameOverTime;

        public GameObject resultPanel;
        public TextMeshProUGUI resultObtainedAetherCoin;
        public TextMeshProUGUI resultMyAetherCoin;

        // egg
        public TextMeshProUGUI eggCountText;
        public TextMeshProUGUI resultObtainedEggCount;
        public TextMeshProUGUI resultMyEggCount;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != null)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            aetherProgressBar.value = 0;
            resultPanel.SetActive(false);
            goToMainButton.onClick.AddListener(() => { MultiplayGameManager.instance.LeaveRoom(); });

            startPanelExitButton.onClick.AddListener(() => { startPanel.SetActive(false); });
        }

        public void SetJoystickCanvasActive(bool isActive)
        {
            joystickCanvas.SetActive(isActive);
        }

        public void SetCurrentEggCount(int count)
        {
            eggCountText.text = count.ToString();
        }

        public void SetCurrentAetherCoinCount(int count)
        {
            aetherCountText.text = count.ToString();
        }

        public void ResetAetherProgressBar()
        {
            aetherProgressBar.value = 0;
        }

        public void SetAetherProgressBar(float value)
        {
            StartCoroutine(FadeSliderValue(value));
        }

        private IEnumerator FadeSliderValue(float targetValue)
        {
            while (true)
            {
                yield return null;
                if (aetherProgressBar.value < targetValue)
                    aetherProgressBar.value += 0.01f;
                else
                    yield break;
            }
        }

        public void UpdateGameTimeText(float gameTime)
        {
            if ((int)gameTime >= 10)
                gameOverTime.text = $"00:{(int)gameTime}";
            else
                gameOverTime.text = $"<color=\"red\">00:0{(int)gameTime}</color>";
        }

        public void ResetGameTimeText()
        {
            gameOverTime.text = "00:00";
        }

        public void SetMultiplayResultPanel(int obtainedCoin, int myCoin, int obtainedEgg, int myEgg)
        {
            resultObtainedAetherCoin.text = obtainedCoin.ToString();
            resultMyAetherCoin.text = myCoin.ToString();
            resultObtainedEggCount.text = obtainedEgg.ToString();
            resultMyEggCount.text = myEgg.ToString();
            resultPanel.SetActive(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BluehatGames
{

    /* API 연동 필요
    - 처음에 서버에서 받아와서 뿌려줌
    - 빼거나 더할 때 서버 API 호출해서 서버에 반영되도록 함 
    */
    public class AetherController : MonoBehaviour
    {
        // AetherController 는 하나만 있는 것이 확실하므로 싱글톤으로 이용
        public static AetherController instance = null;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        // 
        public TextMeshProUGUI aetherCountText;
        private int aetherCount;

        void Start()
        {
            // 로컬에 캐싱하고 있는 에테르의 개수를 가져와서 UI에 출력
            aetherCount = PlayerPrefs.GetInt(PlayerPrefsKey.key_aetherCoin);
            Debug.Log($"current aetherCount => {aetherCount}");
            UpdateText();
        }

        public int GetAetherCount()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKey.key_aetherCoin);
        }

        public void AddAetherCount(int coinCount)
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, coinCount + GetAetherCount());
        }

        // 에테르 개수 차감 
        // TODO: API 연동 필요 
        public void SubAetherCount()
        {
            aetherCount--;
            if (aetherCount > 0)
            {
                UpdateText();
                PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, aetherCount);
            }
            else
            {
                Debug.Log("코인이 부족합니다.");
            }

        }

        void UpdateText()
        {
            aetherCountText.text = aetherCount.ToString();
        }
    }
}

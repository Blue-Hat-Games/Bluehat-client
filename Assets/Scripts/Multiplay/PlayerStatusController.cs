using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/** 멀티플레이에서 플레이어의 상태
- 에테르 에너지
- 에테르 획득 개수
*/
namespace BluehatGames {
public class PlayerStatusController : MonoBehaviour
{
    public TextMeshProUGUI aetherCountText;
    public TextMeshProUGUI gameOverTime;
    public float gameTime;
    public static PlayerStatusController instance = null;
    
    private void Awake()
    {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else if(instance != null) {
            Destroy(this.gameObject);
        }
    }

    // 플레이어끼리 부딪혔을 때 이 에너지를 비교해서 더 작은 쪽의 에너지를 일정량 깎도록 할 것 
    private float aetherEnergy;
    private int aetherCount;
    
    public float addedAetherEnergyValue = 10;
    public int energyToExchangeAether = 50;

    private void Start() {
        aetherCountText.text = aetherCount.ToString();
    }
    private bool isStartTimeAttack = false;
    public void SetStartTimeAttack() {
        isStartTimeAttack = true;
    }

    private float elapsedTime = 0;
    private bool isGameOver = false;
    void Update() {
        if(false == isStartTimeAttack)
            return;
        
        gameTime -= Time.deltaTime;
        if(gameTime < 0) {
            gameOverTime.text = "00:00";
            if(false == isGameOver) {
                isGameOver = true;
                
                int myCoin = PlayerPrefs.GetInt(PlayerPrefsKey.key_aetherCoin);
                SaveAetherCount(myCoin);
                MultiplayUIController.instance.GameOver(GetAetherCount(), myCoin + GetAetherCount());
            }
        } else {
            if((int)gameTime >=10) {
                gameOverTime.text = $"00:{(int)gameTime}";
            } else {
                gameOverTime.text = $"00:0{(int)gameTime}";
            }
            
        }      
    }

    private void SaveAetherCount(int myCoin) {
        Debug.Log($"Save Completed: myCoin = {myCoin}, obtainedCoin = {GetAetherCount()}");
        PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, myCoin + GetAetherCount());
        
    }

    public int GetAetherCount()
    {
        return aetherCount;
    }

    private void SetAetherCount() {
        aetherCount++;
        aetherCountText.text = aetherCount.ToString();
    }

    public float GetAetherEnergy()
    {
        return aetherEnergy;
    }

    public void AddAetherEnergy() {
        
        aetherEnergy += addedAetherEnergyValue;
        float adjustedEnergyValue = aetherEnergy/energyToExchangeAether;
 
        // 교환 가능한 만큼 에너지를 다 모았으면 에너지 초기화, 에테르 획득
        if(aetherEnergy >= energyToExchangeAether) {
            aetherEnergy = 0;
            // UI 설정
            MultiplayUIController.instance.ResetAetherProgressBar();
            SetAetherCount();
        } else {
            // UI 설정
            MultiplayUIController.instance.SetAetherProgressBar(adjustedEnergyValue);
        }
    }
    
    public void SubAetherEnergy(float value) {
        
        aetherEnergy -= value;
        if(aetherEnergy < 0) {
            // 에너지양은 0보다 작아질 수는 없도록 한다.
            aetherEnergy = 0;
        }
    }

}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/** ��Ƽ�÷��̿��� �÷��̾��� ����
- ���׸� ������
- ���׸� ȹ�� ����
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

    // �÷��̾�� �ε����� �� �� �������� ���ؼ� �� ���� ���� �������� ������ �𵵷� �� �� 
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
 
        // ��ȯ ������ ��ŭ �������� �� ������� ������ �ʱ�ȭ, ���׸� ȹ��
        if(aetherEnergy >= energyToExchangeAether) {
            aetherEnergy = 0;
            // UI ����
            MultiplayUIController.instance.ResetAetherProgressBar();
            SetAetherCount();
        } else {
            // UI ����
            MultiplayUIController.instance.SetAetherProgressBar(adjustedEnergyValue);
        }
    }
    
    public void SubAetherEnergy(float value) {
        
        aetherEnergy -= value;
        if(aetherEnergy < 0) {
            // ���������� 0���� �۾��� ���� ������ �Ѵ�.
            aetherEnergy = 0;
        }
    }

}
}
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

    private int aetherCount = 0;
    private float aetherEnergy = 0;
    
    public int energyToExchangeAether = 50;
    public float addedAetherEnergyValue = 10;

    public float gameTime = 0;

    private bool isGameOver = false;
    private bool isStartTimeAttack = false;

    private void Start() {
        MultiplayUIController.instance.SetCurrentAetherCoinCount(aetherCount);
    }


    void Update() {

        if(false == isStartTimeAttack)
            return;
        
        gameTime -= Time.deltaTime;

        if(gameTime < 0) 
        {
           GameOver();
        } 
        else 
        {
            MultiplayUIController.instance.UpdateGameTimeText(gameTime);          
        }      
    }

    public void SetStartTimeAttack() {
        isStartTimeAttack = true;
    }

    private void GameOver()
    {
        // Update���� ���� �� ȣ��� �� �ִ� ��츦 �����ϱ� ����
        if(isGameOver) return;

        MultiplayUIController.instance.ResetGameTimeText();
        
        isGameOver = true;
        // �̹� �ǿ��� ȹ���� ����
        int myCoin = GetMultiplyAetherCount();
        // AetherController�� ���� ȹ�� ���� ����
        AetherController.instance.AddAetherCount(myCoin);
        // UI�� �ݿ�
        MultiplayUIController.instance.SetMultiplayResultPanel(GetMultiplyAetherCount(), myCoin + GetMultiplyAetherCount());
    }

    // ��Ƽ�÷��� ���߿� ��� Aether Count�� ���� ó��
    public int GetMultiplyAetherCount()
    {
        return aetherCount;
    }

    private void AddMultiplayAetherCount() {
        aetherCount++;
        MultiplayUIController.instance.SetCurrentAetherCoinCount(aetherCount);
    }

    // Aether Energy�� ��Ƽ�÷��̿��� �÷��̾� ���� �����̹Ƿ� ���⿡�� ������
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
            AddMultiplayAetherCount();
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
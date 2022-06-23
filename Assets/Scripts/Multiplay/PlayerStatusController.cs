using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** ��Ƽ�÷��̿��� �÷��̾��� ����
- ���׸� ������
- ���׸� ȹ�� ����
*/

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

    // �÷��̾�� �ε����� �� �� �������� ���ؼ� �� ���� ���� �������� ������ �𵵷� �� �� 
    private float aetherEnergy;
    private int aetherCount;
    
    public float addedAetherEnergyValue = 10;

    public int energyToExchangeAether = 50;
    // slider�� 0~1�����ϱ�... ��ü�� ũ�⺰�� ȹ�氡���� �� ������ �ָ� ������
    // �ϴ� ������ �ϱ� ����

    public int GetAetherCount()
    {
        return aetherCount;
    }

    public float GetAetherEnergy()
    {
        return aetherEnergy;
    }

    public void AddAetherEnergy() {
        aetherEnergy += addedAetherEnergyValue;
        float adjustedEnergyValue = aetherEnergy/100;
        // UI ����
        MultiplayUIController.instance.SetAetherProgressBar(adjustedEnergyValue);
        
        // ��ȯ ������ ��ŭ �������� �� ������� ������ �ʱ�ȭ, ���׸� ȹ��
        if(aetherEnergy > energyToExchangeAether) {
            aetherEnergy = 0;
            aetherCount += 1;
        }
    }
    
    public void SubAetherEnergy(float value) {
        
        aetherEnergy -= value;
        if(aetherEnergy < 0) {
            // ���������� 0���� �۾��� ���� ������ �Ѵ�.
            aetherEnergy = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

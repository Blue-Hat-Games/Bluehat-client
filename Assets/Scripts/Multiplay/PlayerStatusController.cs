using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 멀티플레이에서 플레이어의 상태
- 에테르 에너지
- 에테르 획득 개수
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

    // 플레이어끼리 부딪혔을 때 이 에너지를 비교해서 더 작은 쪽의 에너지를 일정량 깎도록 할 것 
    private float aetherEnergy;
    private int aetherCount;
    
    public float addedAetherEnergyValue = 10;

    public int energyToExchangeAether = 50;
    // slider가 0~1까지니까... 물체의 크기별로 획득가능한 걸 차등을 주면 좋은데
    // 일단 빠르게 하기 위해

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
        // UI 설정
        MultiplayUIController.instance.SetAetherProgressBar(adjustedEnergyValue);
        
        // 교환 가능한 만큼 에너지를 다 모았으면 에너지 초기화, 에테르 획득
        if(aetherEnergy > energyToExchangeAether) {
            aetherEnergy = 0;
            aetherCount += 1;
        }
    }
    
    public void SubAetherEnergy(float value) {
        
        aetherEnergy -= value;
        if(aetherEnergy < 0) {
            // 에너지양은 0보다 작아질 수는 없도록 한다.
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

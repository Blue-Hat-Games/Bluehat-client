using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BluehatGames {
public class AetherController : MonoBehaviour
{
    public static AetherController instance = null;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    public TextMeshProUGUI aetherCountText;
    private int aetherCount;

    void Start()
    {
        aetherCount = PlayerPrefs.GetInt(PlayerPrefsKey.key_aetherCoin);
        Debug.Log($"current aetherCount => {aetherCount}");
        aetherCountText.text = aetherCount.ToString();
    }

    public void SubAetherCount() {
        aetherCount--;
        if(aetherCount > 0) {
            aetherCountText.text = aetherCount.ToString();
            PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, aetherCount);
        } else {
            Debug.Log("코인이 부족합니다.");
        }

    }

    void Update()
    {
        
    }
}
}

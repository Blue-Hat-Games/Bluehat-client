using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BluehatGames {
    
    /* API ���� �ʿ�
    - ó���� �������� �޾ƿͼ� �ѷ���
    - ���ų� ���� �� ���� API ȣ���ؼ� ������ �ݿ��ǵ��� �� 
    */
public class AetherController : MonoBehaviour
{
    // AetherController �� �ϳ��� �ִ� ���� Ȯ���ϹǷ� �̱������� �̿�
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

    // 
    public TextMeshProUGUI aetherCountText;
    private int aetherCount;

    void Start()
    {
        // ���ÿ� ĳ���ϰ� �ִ� ���׸��� ������ �����ͼ� UI�� ���
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

    // ���׸� ���� ���� 
    // TODO: API ���� �ʿ� 
    public void SubAetherCount() 
    {
        aetherCount--;
        if(aetherCount > 0) {
            UpdateText();
            PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, aetherCount);
        } else {
            Debug.Log("������ �����մϴ�.");
        }

    }

    void UpdateText()
    {
        aetherCountText.text = aetherCount.ToString();
    }
}
}

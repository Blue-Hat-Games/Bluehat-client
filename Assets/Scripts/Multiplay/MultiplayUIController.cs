using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSwitcher : MonoBehaviour
{
    public Text buttonText;
    public string[] switchTexts;
    public int switchTime;

    void Start()
    {
        StartCoroutine(SwitchText());
    }

    IEnumerator SwitchText()
    {
        int idx = 0;
        while(true)
        {
            buttonText.text = switchTexts[idx%switchTexts.Length];
            yield return new WaitForSeconds(switchTime);
            idx++;
        }
    }

    void Update()
    {
        
    }
}

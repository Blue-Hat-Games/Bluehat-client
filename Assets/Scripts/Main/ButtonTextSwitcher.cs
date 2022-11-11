using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSwitcher : MonoBehaviour
{
    public Text buttonText;
    public string[] switchTexts;
    public int switchTime;

    private void Start()
    {
        StartCoroutine(SwitchText());
    }


    private IEnumerator SwitchText()
    {
        var idx = 0;
        while (true)
        {
            buttonText.text = switchTexts[idx % switchTexts.Length];
            yield return new WaitForSeconds(switchTime);
            idx++;
        }
    }
}
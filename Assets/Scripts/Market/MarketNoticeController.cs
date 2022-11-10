using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketNoticeController : MonoBehaviour
{
    public Text noticeText;
    public string[] noticeTexts;
    public int changeTime;
    public Animator textAnimator;

    void Start()
    {
        StartCoroutine(ShowNoticeText());
    }

    IEnumerator ShowNoticeText()
    {
        int idx = 0;
        while(true)
        {
            yield return null;
            noticeText.text = noticeTexts[idx++];
            textAnimator.SetTrigger("ChangeTrigger");
            if(idx == noticeTexts.Length)
            {
                idx = 0;
            }
            yield return new WaitForSeconds(changeTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

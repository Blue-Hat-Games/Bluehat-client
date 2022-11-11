using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarketNoticeController : MonoBehaviour
{
    public Text noticeText;
    public string[] noticeTexts;
    public int changeTime;
    public Animator textAnimator;

    private void Start()
    {
        StartCoroutine(ShowNoticeText());
    }

    private IEnumerator ShowNoticeText()
    {
        var idx = 0;
        while (true)
        {
            yield return null;
            noticeText.text = noticeTexts[idx++];
            textAnimator.SetTrigger("ChangeTrigger");
            if (idx == noticeTexts.Length) idx = 0;
            yield return new WaitForSeconds(changeTime);
        }
    }
}
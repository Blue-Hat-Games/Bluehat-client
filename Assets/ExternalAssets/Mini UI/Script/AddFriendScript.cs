using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendScript : MonoBehaviour
{
    [SerializeField] private GameObject notification;
    [SerializeField] private Notification notificationScript;
    [SerializeField] private Button addButton;
    [SerializeField] private Text nameText;

    public SlideAnimation slideAnimation;

    private IEnumerator coroutine;

    private string Name;
    private float startPos;
    private void Start()
    {
        coroutine = StartSlideoutCoroutine();
        notification.SetActive(false);
        //addButton.onClick.AddListener(addFriend);
        Name = nameText.text;
        startPos = notification.GetComponent<RectTransform>().anchoredPosition.x;
    }

    void addFriend()
    {
        notification.SetActive(true);
        notificationScript.OutPutNotification(Name);
        slideAnimation.anim.Play("SlideIn");
        Invoke("StopSlide", 3);
    }

    private IEnumerator StartSlideoutCoroutine()
    {       
        yield return new WaitForSeconds(3);
        slideAnimation.anim.Play("SlideOut");
        StopSlide();       
    }

    void StopSlide()
    {
        slideAnimation.anim.Play("SlideOut");
    }
}

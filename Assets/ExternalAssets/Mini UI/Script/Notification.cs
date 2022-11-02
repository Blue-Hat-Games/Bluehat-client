using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public Text outPutText;

    public void OutPutNotification(string text)
    {
        outPutText.text = text + "  Is Now Your Friend";
    }
}

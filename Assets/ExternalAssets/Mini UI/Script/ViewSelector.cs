using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSelector : MonoBehaviour
{
    public Button AchievementViewButton;
    public Button OtherViewButton;

    public GameObject AchievementViewPanel;
    public GameObject OtherViewPanel;

    public void Start()
    {
        AchievementViewButton.onClick.AddListener(Achievement);
        OtherViewButton.onClick.AddListener(OtherView);

        AchievementViewPanel.SetActive(true);
        OtherViewPanel.SetActive(false);
    }
    public void Achievement()
    {
        AchievementViewPanel.SetActive(true);
        OtherViewPanel.SetActive(false);
    }

    public void OtherView()
    {
        OtherViewPanel.SetActive(true);
        AchievementViewPanel.SetActive(false);
    }


}

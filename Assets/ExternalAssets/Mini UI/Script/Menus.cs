using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Menus : MonoBehaviour
{

    [SerializeField] private GameObject lightStore;
    [SerializeField] private GameObject lightMaps;
    [SerializeField] private GameObject lightItems;
    [SerializeField] private GameObject lightMessages;
    [SerializeField] private GameObject lightRanking;
    [SerializeField] private GameObject lightSettings;
    [SerializeField] private GameObject lightMission;
    [SerializeField] private GameObject lightAchievements;
    [SerializeField] private GameObject lightLevelSelect;
    [SerializeField] private GameObject lightRewards;

    [Space(10)]
    [SerializeField] private GameObject darkStore;
    [SerializeField] private GameObject darkMaps;
    [SerializeField] private GameObject darkItems;
    [SerializeField] private GameObject darkMessages;
    [SerializeField] private GameObject darkRanking;
    [SerializeField] private GameObject darkSettings;
    [SerializeField] private GameObject darkMission;
    [SerializeField] private GameObject darkAchievements;
    [SerializeField] private GameObject darkLevelSelect;
    [SerializeField] private GameObject darkRewards;

    [Space(10)]
    [SerializeField] private GameObject lightLogin;
    [SerializeField] private GameObject darkLogin;
    [SerializeField] private GameObject DemoSelect;
    [SerializeField] private GameObject LightTheme;
    [SerializeField] private GameObject DarkTheme;

    [Space(10)]
    [SerializeField] private Button lightStoreButton;
    [SerializeField] private Button lightMapsButton;
    [SerializeField] private Button lightItemsButton;
    [SerializeField] private Button lightMessagesButton;
    [SerializeField] private Button lightRankingButton;
    [SerializeField] private Button lightSettingsButton;
    [SerializeField] private Button lightMissionButton;
    [SerializeField] private Button lightLoginButton;
    [SerializeField] private Button lightAchievementsButton;
    [SerializeField] private Button lightLevelSelectButton;
    [SerializeField] private Button lightRewardsButton;

    [Space(10)]
    [SerializeField] private Button DarkButton;
    [SerializeField] private Button LightButton;

    [Space(10)]
    [SerializeField] private Button darkStoreButton;
    [SerializeField] private Button darkMapsButton;
    [SerializeField] private Button darkItemsButton;
    [SerializeField] private Button darkMessagesButton;
    [SerializeField] private Button darkRankingButton;
    [SerializeField] private Button darkSettingsButton;
    [SerializeField] private Button darkMissionButton;
    [SerializeField] private Button darkLoginButton;
    [SerializeField] private Button darkAchievementsButton;
    [SerializeField] private Button darkLevelSelectButton;
    [SerializeField] private Button darkRewardsButton;

    [Space(10)]
    [SerializeField] private Button BackToDemoSelectButton;
   

    [Space(10)]
     public GameObject MenuDark;
     public GameObject MenuLight;
     public string themeColor;


    public LoadingScript loadingScript;
    public StartScript startScript;

    [Space(10)]
    
    
    public GameObject backButton;

    [SerializeField] private GameObject Panel;
    void Start()
    {
        
        DarkButton.onClick.AddListener(OpenDarkTheme);
        LightButton.onClick.AddListener(OpenLightTheme);

        lightStoreButton.onClick.AddListener(LightStore);
        lightMapsButton.onClick.AddListener(LightMaps);
        lightItemsButton.onClick.AddListener(LightItems);
        lightMessagesButton.onClick.AddListener(LightMessages);
        lightRankingButton.onClick.AddListener(LightRanking);
        lightSettingsButton.onClick.AddListener(LightSettings);
        lightMissionButton.onClick.AddListener(LightMission);
        lightLevelSelectButton.onClick.AddListener(LightLevelSelect);
        lightAchievementsButton.onClick.AddListener(LightAchievements);
        lightRewardsButton.onClick.AddListener(LightDailyRewards);

        darkStoreButton.onClick.AddListener(DarkStore);
        darkMapsButton.onClick.AddListener(DarkMaps);
        darkItemsButton.onClick.AddListener(DarkItems);
        darkMessagesButton.onClick.AddListener(DarkMessages);
        darkRankingButton.onClick.AddListener(DarkRanking);
        darkSettingsButton.onClick.AddListener(DarkSettings);
        darkMissionButton.onClick.AddListener(DarkMission);
        darkLevelSelectButton.onClick.AddListener(DarkLevelSelect);
        darkAchievementsButton.onClick.AddListener(DarkAchievements);
        darkRewardsButton.onClick.AddListener(darkDailyRewards);

        lightLoginButton.onClick.AddListener(LightLogin);
        darkLoginButton.onClick.AddListener(DarkLogin);

        BackToDemoSelectButton.onClick.AddListener(BackButton);
        backButton.SetActive(false);
    }

    void LightLogin()
    {
        Panel.SetActive(true);
        lightLogin.SetActive(true);
    }

    void DarkLogin()
    {
        Panel.SetActive(true);
        darkLogin.SetActive(true);
    }

    void OpenDarkTheme()
    {
        themeColor = "Dark";
        DemoSelect.SetActive(false);
        MenuDark.SetActive(true);
        startScript.gemsAndCoin.SetActive(true);
        backButton.SetActive(true);
    }

    void OpenLightTheme()
    {
        themeColor = "Light";
        DemoSelect.SetActive(false);
        MenuLight.SetActive(true);
        startScript.gemsAndCoin.SetActive(true);
        backButton.SetActive(true);
    }

    void LightDailyRewards()
    {
        Panel.SetActive(true);
        lightRewards.SetActive(true);
    }
    void LightStore()
    {
        Panel.SetActive(true);
        lightStore.SetActive(true);
    }

    void LightMaps()
    {
        Panel.SetActive(true);
        lightMaps.SetActive(true);
        Debug.Log(" Maps");
    }

    void LightItems()
    {
        Panel.SetActive(true);
        lightItems.SetActive(true);
        Debug.Log(" Items");
    }

    void LightMessages()
    {
        Panel.SetActive(true);
        lightMessages.SetActive(true);
        Debug.Log(" Messages");
    }

    void LightRanking()
    {
        Panel.SetActive(true);
        lightRanking.SetActive(true);
        Debug.Log(" Ranking");
    }

    void LightSettings()
    {
        Panel.SetActive(true);
        lightSettings.SetActive(true);
        Debug.Log(" Settings");
    }

    void LightMission()
    {
        Panel.SetActive(true);
        lightMission.SetActive(true);
        Debug.Log(" Mission");
    }

    void LightLevelSelect()
    {
        Panel.SetActive(true);
        lightLevelSelect.SetActive(true);
    }

    void LightAchievements()
    {
        Panel.SetActive(true);
        lightAchievements.SetActive(true);
    }

    //---------------------------------------------------------------------------------------------
    //-------------------------------DARK THEME---------------------------------------------------------
    void darkDailyRewards()
    {
        Panel.SetActive(true);
        darkRewards.SetActive(true);
    }
    void DarkAchievements()
    {
        Panel.SetActive(true);
        darkAchievements.SetActive(true);
    }




    void DarkLevelSelect()
    {
        Panel.SetActive(true);
        darkLevelSelect.SetActive(true);
    }


    void DarkStore()
    {
        Panel.SetActive(true);
        darkStore.SetActive(true);
        Debug.Log(" Store");
    }

    void DarkMaps()
    {
        Panel.SetActive(true);
        darkMaps.SetActive(true);
        Debug.Log(" Maps");
    }

    void DarkItems()
    {
        Panel.SetActive(true);
        darkItems.SetActive(true);
        Debug.Log(" Items");
    }

    void DarkMessages()
    {
        Panel.SetActive(true);
        darkMessages.SetActive(true);
        Debug.Log(" Messages");
    }

    void DarkRanking()
    {
        Panel.SetActive(true);
        darkRanking.SetActive(true);
        Debug.Log(" Ranking");
    }

    void DarkSettings()
    {
        Panel.SetActive(true);
        darkSettings.SetActive(true);
        Debug.Log(" Settings");
    }

    void DarkMission()
    {
        Panel.SetActive(true);
        darkMission.SetActive(true);
        Debug.Log(" Mission");
    }

    void BackButton()
    {
        loadingScript.loadingProgress.value = 0;
        loadingScript.loadValue = 0.0f;
        themeColor = " ";
        DemoSelect.SetActive(true);
        MenuLight.SetActive(false);
        MenuDark.SetActive(false);
        startScript.gemsAndCoin.SetActive(false);
        startScript.startingPage.SetActive(false);
        backButton.SetActive(false);
    }
}

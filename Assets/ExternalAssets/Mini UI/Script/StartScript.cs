using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public GameObject loading;
    public LoadingScript loadingScript;
    public Button play;
    public GameObject startingPage;
    public GameObject gemsAndCoin;
    public GameObject DemoSelect;
    public Menus menuScript;
    

    private void Start()
    {
        play.onClick.AddListener(StartButton);
    }
    public void StartButton()
    {
        loading.SetActive(true);
        StartCoroutine(loadingScript.loading());
        startingPage.SetActive(false);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public StartScript startScript;
    public Slider loadingProgress;
    public float loadValue;
    public Menus menusScript;
    public Text loadText;
    

    private void Start()
    {
        loadValue = 0.0f;
        
    }

    public IEnumerator loading()
    {
        while (loadingProgress.value < 100f)
        {           
            loadValue += 0.5f;

            loadingProgress.value = loadValue;
            loadText.text = loadValue.ToString();
            if (loadingProgress.value == 100f)
            {
                StopCoroutine(loading());
                //startScript.gemsAndCoin.SetActive(true);
                startScript.DemoSelect.SetActive(true);
                startScript.loading.SetActive(false);
                //menusScript.backButton.SetActive(true);

            }
            yield return null;
        }           
    }

    //public void SendTheme(string theme)
    //{
    //    if (theme == "Light")
    //    {
    //       menusScript.MenuLight.SetActive(true);
    //       menusScript.backButton.SetActive(true);
    //    }
    //    else if (theme == "Dark")
    //    {
    //       Debug.Log("from dark");
    //       menusScript.MenuDark.SetActive(true);
    //       menusScript.backButton.SetActive(true);
    //    }
    //}
}

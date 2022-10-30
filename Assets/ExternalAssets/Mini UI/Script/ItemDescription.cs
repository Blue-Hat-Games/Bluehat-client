using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    public List<GameObject> objectList;
    public GameObject itemDescriptionWindow;
    public bool windowIsActive;
    
    private void Awake()
    {
        windowIsActive = false;
    }

    public  void ShowItemDescription(string itemSelected)
    {
        foreach (GameObject gameObject in objectList)
        {
            if (itemSelected == gameObject.name )
            {
                Debug.Log(gameObject.name + " ");
                if (windowIsActive == false) {
                    itemDescriptionWindow.SetActive(true);
                    gameObject.SetActive(true);
                    windowIsActive = true;
                }               
            }                        
        }        
    }
}

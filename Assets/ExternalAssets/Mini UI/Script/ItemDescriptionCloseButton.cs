using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescriptionCloseButton : MonoBehaviour
{
    public ItemDescription itemDescription;
     
    public void Close()
    {
        foreach (GameObject gameObject in itemDescription.objectList)
        {
            gameObject.SetActive(false);
        }
        itemDescription.windowIsActive = false;
        itemDescription.itemDescriptionWindow.SetActive(false);
    }
}

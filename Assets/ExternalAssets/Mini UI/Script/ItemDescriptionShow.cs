using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemDescriptionShow : MonoBehaviour
{
    public ItemDescription itemDescription;
    public void PressButton()
    {
        if (itemDescription.windowIsActive == false)
        {
            string itemName = this.name;
            itemDescription.ShowItemDescription(itemName);
        } else
        {

        }
        
        //Debug.Log(itemName);
        


    }
}

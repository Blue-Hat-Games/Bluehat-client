using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private GameObject NormalButton;
    [SerializeField] private GameObject PressedButton;
    

    public void OnPointerDown(PointerEventData eventData)
    {
        NormalButton.SetActive(false);
        PressedButton.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        NormalButton.SetActive(true);
        PressedButton.SetActive(false);
        Pressed();
    }

    public void Pressed()
    {
        Debug.Log(" Called From Press Up");
    }

}

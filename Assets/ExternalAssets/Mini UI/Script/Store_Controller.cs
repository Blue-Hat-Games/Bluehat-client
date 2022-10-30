using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Store_Controller : MonoBehaviour  ,IPointerDownHandler, IPointerUpHandler
{
    public GameObject Coin_view;
    public GameObject Gem_view;

    public GameObject Gems_On_Image;
    public GameObject Gems_Pressed_Image;
    public GameObject Gems_Off_Image;
    public GameObject Coins_On_Image;
    public GameObject Coins_Pressed_Image;
    public GameObject Coins_Off_Image;

    public Button gemButton;
    public Button coinButton;
    // Start is called before the first frame update
    void Start()
    {

        Coin_view.SetActive(false);
        Gem_view.SetActive(true);
        Gems_Off_Image.SetActive(false);
        Coins_On_Image.SetActive(false);
        Gems_On_Image.SetActive(false);
        Coins_Off_Image.SetActive(true);
        Coins_Pressed_Image.SetActive(false);
        Gems_Pressed_Image.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GemButton()
    {
        Coins_Off_Image.SetActive(true);
        Coins_On_Image.SetActive(false);
        Coins_Pressed_Image.SetActive(false);
        //Gems_On_Image.SetActive(true);
        Gems_Pressed_Image.SetActive(true);
        Gems_Off_Image.SetActive(false);

        Gem_view.SetActive(true);
        Coin_view.SetActive(false);
    }

    public void CoinButton()
    {
        Gems_On_Image.SetActive(false);
        Gems_Off_Image.SetActive(true);
        Gems_Pressed_Image.SetActive(false);

        Coins_Pressed_Image.SetActive(true);
        //Coins_On_Image.SetActive(true);
        Coins_Off_Image.SetActive(false);

        Coin_view.SetActive(true);
        Gem_view.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("from pointer down");

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}

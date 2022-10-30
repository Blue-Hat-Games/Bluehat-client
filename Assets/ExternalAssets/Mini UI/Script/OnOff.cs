using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour
{
    [SerializeField] private GameObject textOFF;
    [SerializeField] private GameObject textON;
    [SerializeField] private Switch_Slide switchSlide;

    private void Start()
    {
        textON.SetActive(false);
    }

    private void Update()
    {
        if(switchSlide.textIsOn == true)
        {
            SetOn();
        } else
        {
            SetOff();
        }
    }
    public void SetOn()
    {
        textON.SetActive(true);
        textOFF.SetActive(false);
    }

    public void SetOff()
    {
        textON.SetActive(false);
        textOFF.SetActive(true);
    }
}

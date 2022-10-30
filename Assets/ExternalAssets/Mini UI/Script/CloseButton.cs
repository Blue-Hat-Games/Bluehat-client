using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public GameObject Panel;
    void Start()
    {
        
    }

    public void Close()
    {
        Panel.SetActive(false);
        this.gameObject.SetActive(false);
    }
}

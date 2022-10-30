using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class Map : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    [SerializeField] private GameObject MapObj;
    [SerializeField] private RectTransform Viewport_Dimensions;
    private Vector2 Half_Map_Background_Dimensions;
    private Vector2 Half_Viewport_Dimensions;
    private float VerticalPixel;
    private float HorizontalPixel;
    private RectTransform Map_Background_Dimensions;

    //private float deltaX, deltaY;

    //private float ValueToClampY;
    private GameObject raycastTest;

    private RectTransform rectr;

    public void Awake()
    {
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {       
        raycastTest = eventData.pointerCurrentRaycast.gameObject;
        try
        {
            if (raycastTest.name.Contains("Map"))
            {
                rectr = raycastTest.GetComponent<RectTransform>();
                rectr.anchoredPosition += eventData.delta;
            }
        }

        catch (NullReferenceException ex)
        {
            Debug.Log("out of bounds");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    // Start is called before the first frame update

    public void OnPointerDown(PointerEventData eventData)
    {

    }
    void Start()
    {
        Map_Background_Dimensions = MapObj.GetComponent<RectTransform>();
        Map_Background_Dimensions.anchoredPosition = new Vector2(0, 0);
        Half_Map_Background_Dimensions = new Vector2 ((Map_Background_Dimensions.rect.width / 2) , (Map_Background_Dimensions.rect.height / 2));
        Half_Viewport_Dimensions = new Vector2((Viewport_Dimensions.rect.width / 2), (Viewport_Dimensions.rect.height / 2));
        VerticalPixel = Half_Map_Background_Dimensions.y - Half_Viewport_Dimensions.y;
        HorizontalPixel = Half_Map_Background_Dimensions.x - Half_Viewport_Dimensions.x;
        Debug.Log(VerticalPixel + "  " + HorizontalPixel);
    }

    // Update is called once per frame
    void Update()
    {
        Map_Background_Dimensions.anchoredPosition = new Vector2(Mathf.Clamp(Map_Background_Dimensions.anchoredPosition.x, -HorizontalPixel, HorizontalPixel), Mathf.Clamp(Map_Background_Dimensions.anchoredPosition.y,-VerticalPixel,VerticalPixel));
    }
}


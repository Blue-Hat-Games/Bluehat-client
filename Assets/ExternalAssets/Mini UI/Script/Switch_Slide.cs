using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch_Slide : MonoBehaviour
{

    [SerializeField] private Button btn1;
    [SerializeField] private Image Handle_Image_Off, Handle_Image_On;
    [SerializeField] private Image Background_Image_Off, Background_Image_On;
    [SerializeField] private GameObject Background_Pixel_Dimension;
    [SerializeField] private GameObject Toggle_Pixel_Dimension;
    [SerializeField] private float Offset_Pixel;
    


    public bool textIsOn;

    private float Toggle_Height;
    private float Toggle_Size;
    private float Toggle_Center;
    private float BG_StartPosition;
    private float BG_Size;
    private float BG_Center;
    
    Color currentcolor;
    Color newColor;

    Color currentcolor2;
    Color newColor2;

    public bool isoff;

    private float time = 0;

    private Vector2 Start_Point;
    private Vector2 End_Point;

    // Start is called before the first frame update
    void Start()
    {

        

        currentcolor = new Color(Handle_Image_Off.color.r, Handle_Image_Off.color.g, Handle_Image_Off.color.b, 1);

        newColor = new Color(Handle_Image_Off.color.r, Handle_Image_Off.color.g, Handle_Image_Off.color.b, 0);

        currentcolor2 = new Color(Handle_Image_On.color.r, Handle_Image_On.color.g, Handle_Image_On.color.b, 1);

        newColor2 = new Color(Handle_Image_On.color.r, Handle_Image_On.color.g, Handle_Image_On.color.b, 0);

        Handle_Image_On.color = newColor2;

        isoff = true; //Default state is off
        
        Toggle_Size = Toggle_Pixel_Dimension.GetComponent<RectTransform>().rect.width; //Width of Toggle in pixels

        Toggle_Height = Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.y;

        Debug.Log("Toggle Height " + Toggle_Height);

        Toggle_Center = Toggle_Size / 2; // Center Point of the Toggle

        BG_Size = Background_Pixel_Dimension.GetComponent<RectTransform>().rect.width; //Width of Background in pixels

        BG_Center = BG_Size / 2; // The half size of the Background.

        BG_StartPosition = BG_Center - (Offset_Pixel + Toggle_Center); // The starting position of the handle, default is on off when it is first run.

        Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition = new Vector2(-BG_StartPosition, Toggle_Height); //Set the handle to the off position.

        Start_Point = new Vector2(-BG_StartPosition, Toggle_Height);

        End_Point = new Vector2(BG_StartPosition, Toggle_Height);

        Debug.Log(End_Point + "  " + Start_Point);

    }

    // Update is called once per frame
    void Update()
    {
  
    }


    //This is the function called when we click the button.
    void Execute1()
    {

    }



    
    //Base on the state of the switch we start a coroutine to handle the movement of the toggle handle
    public void Switching()
    {         
              
        if (isoff)
        {
            textIsOn = true;
            time = 0;
            StartCoroutine(SwitchCoroutineOn());
            btn1.interactable = false;
            isoff = false;            
        }
        else  
        {
            textIsOn = false;
            time = 0;
            StartCoroutine(SwitchCoroutineOff());
            btn1.interactable = false;
            isoff = true;
        }
    }


    private IEnumerator SwitchCoroutineOn()
    {
        while (time < 1f)
        {
            time += 0.02f;

            Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(Start_Point, End_Point, time);

            Handle_Image_Off.color = Color.Lerp(currentcolor, newColor, time);

            Handle_Image_On.color = Color.Lerp(newColor2, currentcolor2, time);

            if (Mathf.Round(Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.x) == End_Point.x)
            {
                Execute1();
                Debug.Log("From on");
                btn1.interactable = true;
                StopCoroutine(SwitchCoroutineOn());
            }
            yield return null;           
        }
    }

    private IEnumerator SwitchCoroutineOff()
    {
        while (time < 1f)
        {
            time += 0.02f;

            Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(End_Point, Start_Point, time);

            Handle_Image_Off.color = Color.Lerp(newColor, currentcolor, time);

            Handle_Image_On.color = Color.Lerp(currentcolor2, newColor2, time);

            if (Mathf.Round(Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.x) == -End_Point.x)
            {
                btn1.interactable = true;
                StopCoroutine(SwitchCoroutineOff());
            }
            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool Pressed;
    public Animator buttonAnim;

    private string ANIM_PARAMETER_ISPRESSED = "IsPressed";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // press animation이 계속 두 번 재생되는 문제 있음 
        // buttonAnim.SetTrigger(ANIM_PARAMETER_ISPRESSED);
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Pressed = false;
    }

}

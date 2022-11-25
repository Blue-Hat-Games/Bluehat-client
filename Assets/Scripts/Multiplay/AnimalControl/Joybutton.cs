using UnityEngine;
using UnityEngine.EventSystems;

public class Joybutton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector] public bool Pressed;

    public Animator buttonAnim;


    public void OnPointerDown(PointerEventData eventData)
    {
        // press animation이 계속 두 번 재생되는 문제 있음 
        // buttonAnim.SetTrigger(ANIM_PARAMETER_ISPRESSED);
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
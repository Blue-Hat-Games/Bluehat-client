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
        // press animation�� ��� �� �� ����Ǵ� ���� ���� 
        // buttonAnim.SetTrigger(ANIM_PARAMETER_ISPRESSED);
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Pressed = false;
    }

}

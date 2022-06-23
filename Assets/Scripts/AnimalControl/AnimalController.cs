using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    protected Joystick joystick;
    protected Joybutton joybutton;

    protected bool jump;

    public float moveSpeed;
    public float jumpPower;
    public float rotSpeed;

    private Rigidbody rigidbody;
    private Animator animator;
    private Collider collider;
    
    private float distToGround;

    private string ANIM_PARAMETER_JUMP = "Jump";
    private string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";
    
    public float isGroundedRayDistance;

    
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();

        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        collider = GetComponent<Collider>();

        distToGround = collider.bounds.extents.y;
        Debug.Log($"distToGround => {distToGround}");
    }

    void Update()
    {
        animator.SetFloat(ANIM_PARAMETER_MOTIONSPEED, joystick.InputScale);
        // 이동 방향으로 회전시켜주자  
        rigidbody.velocity = new Vector3(joystick.Horizontal * moveSpeed, rigidbody.velocity.y, joystick.Vertical * moveSpeed);
        
        var h = joystick.Horizontal;
        var v = joystick.Vertical;

        Vector3 dir = new Vector3(h, 0, v);

        if(!(h == 0 && v == 0))
        {
            // 바라보는 방향으로 회전
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotSpeed);
        }

        // Jump에 대한 처리
        if(!jump && joybutton.Pressed) {
            jump = true;
            rigidbody.velocity = Vector3.up * jumpPower;
            animator.SetTrigger(ANIM_PARAMETER_JUMP);
        }

        if(jump && !joybutton.Pressed) {
            jump = false;
        }
    }

    // 플레이어가 바닥에 붙어있는지 확인하기 위해 Raycast 이용
    private bool IsGrounded() {
        var isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + isGroundedRayDistance);
        Debug.Log($"IsGrounded = {isGrounded}");
        return isGrounded;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayAnimalController : MonoBehaviourPun, IPunObservable
{
    private float camAngle;

    // 받은 데이터 기억 변수 (보간처리하기 위해서)
    Vector3 remotePos = Vector3.zero;
    Quaternion remoteRot = Quaternion.identity;
    Quaternion remoteCamRot = Quaternion.identity;
    Vector3 remoteScale = Vector3.zero;

    protected Joystick joystick;
    protected Joybutton joybutton;

    private float moveSpeed = 10;
    private float jumpPower = 6;
    public float rotSpeed = 10;

    private Rigidbody rigid;
    private Animator animator;

    private string ANIM_PARAMETER_JUMP = "Jump";
    private string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";

    private bool isGround = true;

    private MultiplayCameraController multiplayCameraController;
    Quaternion _initialRotation;
    public float minClamp = -90.0f;
    public float maxClamp = 90f;
    float _pitch, _yaw;
    float prevH, prevV;

    public Vector3 prevCamForwardVector;
    private GameObject testSphere;

    void Start()
    {
        testSphere = GameObject.Find("TestSphere").gameObject;

        prevCamForwardVector = Vector3.zero;
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();

        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        _initialRotation = this.transform.rotation;
        multiplayCameraController = GameObject.FindObjectOfType<MultiplayCameraController>();
        prevH = 0;
        prevV = 0;
    }

    void Update()
    {
        // 리모트 캐릭터 처리
        if (photonView.IsMine == false)
        {
            ControlRemotePlayer();
            return;
        }

        // 애니메이터 파라미터 설정 
        animator.SetFloat(ANIM_PARAMETER_MOTIONSPEED, joystick.InputScale);
        // 이동 방향으로 회전  
        


        // 조이스틱 h, v가 0이 아닐 때
        // - 값이 이전과 다르게 변했으면 그 방향으로 이동시켜줌
        // - 값이 이전과 같으면 그 방향으로 이동시켜줌
        // - 그런데 이 때 touch delta position에 변화가 있으면 deltaPos에 맞게 이동 방향을 바꿔줌




        var h = joystick.Horizontal;
        var v = joystick.Vertical;

        Vector3 dir = new Vector3(h, 0, v);
        Debug.Log($"joystick h = {h}, v = {v}");
        Debug.Log($"joystick dir = {dir}");
        // rigid.velocity = new Vector3(h * moveSpeed, rigid.velocity.y, v * moveSpeed);
        
        if (!(h == 0 && v == 0)) // 조이스틱에 값이 들어오고 있을 때 
        {
            Transform CameraTr = multiplayCameraController.GetCameraTransform();
            
            Vector3 lookForward = new Vector3(CameraTr.forward.x, 0f, CameraTr.forward.z).normalized;
            Vector3 lookRight = new Vector3(CameraTr.right.x, 0f, CameraTr.right.z).normalized;
            Vector3 moveDir = lookForward * v + lookRight * h;
            Debug.Log($"moveDir => {moveDir}");
            this.transform.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 5f;

            return;

        }
        else
        {
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
            
        }

        prevH = h;
        prevV = v;

        // Jump에 대한 처리
        if (isGround && joybutton.Pressed)
        {
            rigid.velocity = Vector3.up * jumpPower;
            animator.SetTrigger(ANIM_PARAMETER_JUMP);
            isGround = false;
        }

    }

    private void ControlRemotePlayer()
    {
        transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
        Debug.Log($"ControlRemotePlayer | position = {transform.position}, remotePos = {remotePos}");
        transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
    }

    // IPunObservable 상속 시 꼭 구현해야 하는 것
    // - 데이터를 네트워크 사용자 간에 보내고 받고 하게 하는 콜백 함수
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 내가 데이터를 보내는 중이라면
        if (stream.IsWriting) // 내꺼보내는 거
        {
            // 이 방안에 있는 모든 사용자에게 브로드캐스트 
            // - 내 포지션 값을 보내보자
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
        }
        // 내가 데이터를 받는 중이라면 
        else // 원격에 있는 나 
        {
            // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
            remotePos = (Vector3)stream.ReceiveNext();
            remoteRot = (Quaternion)stream.ReceiveNext();
            remoteScale = (Vector3)stream.ReceiveNext();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 부딪힌 물체의 태그가 "Ground"라면
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGround를 true로 변경
            isGround = true;
        }
    }
}

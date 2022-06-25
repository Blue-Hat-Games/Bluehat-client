using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimalController : MonoBehaviourPun, IPunObservable
{
    public Transform camera;
    private float camAngle;
    
    // ���� ������ ��� ���� (����ó���ϱ� ���ؼ�)
    Vector3 remotePos = Vector3.zero;
    Quaternion remoteRot = Quaternion.identity;
    Quaternion remoteCamRot = Quaternion.identity;

    protected Joystick joystick;
    protected Joybutton joybutton;

    protected bool jump;

    public float moveSpeed;
    public float jumpPower;
    public float rotSpeed;

    private Rigidbody rigidbody;
    private Animator animator;
 
    private string ANIM_PARAMETER_JUMP = "Jump";
    private string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";
    
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();

        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // ����Ʈ ĳ���� ó��
        if(photonView.IsMine == false) {
            ControlRemotePlayer();
            return;
        }

        // �ִϸ����� �Ķ���� ���� 
        animator.SetFloat(ANIM_PARAMETER_MOTIONSPEED, joystick.InputScale);
        // �̵� �������� ȸ��  
        rigidbody.velocity = new Vector3(joystick.Horizontal * moveSpeed, rigidbody.velocity.y, joystick.Vertical * moveSpeed);
        
        var h = joystick.Horizontal;
        var v = joystick.Vertical;

        Vector3 dir = new Vector3(h, 0, v);

        if(!(h == 0 && v == 0))
        {
            // �ٶ󺸴� �������� ȸ��
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotSpeed);
        }

        // Jump�� ���� ó��
        if(!jump && joybutton.Pressed) {
            jump = true;
            rigidbody.velocity = Vector3.up * jumpPower;
            animator.SetTrigger(ANIM_PARAMETER_JUMP);
        }

        if(jump && !joybutton.Pressed) {
            jump = false;
        }
    }

    private void ControlRemotePlayer() {

        transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
        //camera.rotation = Quaternion.Lerp(camera.rotation, remoteCamRot, 10 * Time.deltaTime);
    }

    // IPunObservable ��� �� �� �����ؾ� �ϴ� ��
    // - �����͸� ��Ʈ��ũ ����� ���� ������ �ް� �ϰ� �ϴ� �ݹ� �Լ�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� �����͸� ������ ���̶��
        if(stream.IsWriting) // ���������� ��
        {
            // �� ��ȿ� �ִ� ��� ����ڿ��� ��ε�ĳ��Ʈ 
            // - �� ������ ���� ��������
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(camera.rotation);
        }
        // ���� �����͸� �޴� ���̶�� 
        else // ���ݿ� �ִ� �� 
        {
            // ������� ������ ������� ����. �ٵ� Ÿ��ĳ���� ���־�� ��
            remotePos = (Vector3)stream.ReceiveNext();
            remoteRot = (Quaternion)stream.ReceiveNext();
            //remoteCamRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

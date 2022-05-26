using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// ���콺 �Է¿� ���� ī�޶�� ���Ϸθ� ȸ��
// �¿�� ��ü ȸ���ϰ� �ʹ�
// ����� �Է¿� ���� �̵��ϰ� �ʹ�
// ��Ʈ��ũ �÷��̾� ���� ������ ����ȭ ó���� �ϰ� �ʹ�.
public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public Transform camera;

    // �ʿ�Ӽ�
    public float speed = 5;
    public float jumpPower = 10;
    public float gravity = -20;
    public float rotSpeed = 10;
    float yVelocity = 0;
    float camAngle; // ���Ϸθ�
    float bodyAngle; // �¿�θ� eulerangles�� y��

    CharacterController cc;
    void Start()
    {
        cc = GetComponent<CharacterController>();        
    }

    void Update()
    {
        // �� �ڽ��� �ƴ϶�� �Է� ����
        if (false == photonView.IsMine)
            return;
        
        // ���콺 �Է¿� ���� ī�޶�� ���Ϸθ� ȸ��
        RotateCamera();
        // �¿�� ��ü ȸ���ϰ� �ʹ�
        RotateBody();
        // ����� �Է¿� ���� �̵��ϰ� �ʹ�
        Move();
    }

    private void RotateCamera()
    {
        float value = Input.GetAxis("Mouse Y");
        camAngle += value * rotSpeed * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -60, 60);

        // Camera�� �÷��̾��� �ڽ��̹Ƿ� ���� ���ؿ��� ������ �� 
        camera.localEulerAngles = new Vector3(-camAngle, 0, 0);
        
    }

    private void RotateBody()
    {
        float value = Input.GetAxis("Mouse X");
        bodyAngle += value * rotSpeed * Time.deltaTime;

        // �¿�� �÷��̾� ��ü�� ȸ�� 
        transform.eulerAngles = new Vector3(0, bodyAngle, 0);
    }

    private void Move()
    {
        // 1. ����� �Է¿� ���� 
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // 2. ������ �ʿ�
        Vector3 dir = new Vector3(h, 0, v) * speed; // speed�� ���ؼ� ������ ���̱���
        // -> ī�޶� �ٶ󺸴� �������� ���� ��ȯ 
        dir = camera.TransformDirection(dir);

        // �ٴڿ� ������ ���� �ӵ��� 0���� ���� (�����׷�)
        if(cc.isGrounded)
        {
            yVelocity = 0;
        }
        // ����
        if(Input.GetButtonDown("Jump"))
        {
            yVelocity = jumpPower;
        }
        // �߷��� �����ϰ� �ʹ�. v = v0 + at
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;         
        // 3. �̵��ϰ� �ʹ� p = p0 + vt
        cc.Move(dir * Time.deltaTime);
    }

    // IPunObservable ��� �� �� �����ؾ� �ϴ� ��
    // - �����͸� ��Ʈ��ũ ����� ���� ������ �ް� �ϰ� �ϴ� �ݹ� �Լ�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� �����͸� ������ ���̶��
        if(stream.IsWriting)
        {
            // �� ��ȿ� �ִ� ��� ����ڿ��� ��ε�ĳ��Ʈ 
            // - �� ������ ���� ��������
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(camera.rotation);
        }
        // ���� �����͸� �޴� ���̶�� 
        else
        {
            // ������� ������ ������� ����. �ٵ� Ÿ��ĳ���� ���־�� ��
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            camera.rotation = (Quaternion)stream.ReceiveNext(); 
        }
    }
}

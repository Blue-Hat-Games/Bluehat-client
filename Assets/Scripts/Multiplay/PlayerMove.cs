using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 마우스 입력에 따라 카메라는 상하로만 회전
// 좌우는 몸체 회전하고 싶다
// 사용자 입력에 따라 이동하고 싶다
// 네트워크 플레이어 간의 데이터 동기화 처리를 하고 싶다.
public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public Transform playerCamera;

    // 필요속성
    public float speed = 5;
    public float jumpPower = 10;
    public float gravity = -20;
    public float rotSpeed = 10;
    float yVelocity = 0;
    float camAngle; // 상하로만
    float bodyAngle; // 좌우로만 eulerangles의 y만

    CharacterController cc;

    // 받은 데이터 기억 변수 (보간처리하기 위해서)
    Vector3 remotePos = Vector3.zero;
    Quaternion remoteRot = Quaternion.identity;
    Quaternion remoteCamRot = Quaternion.identity;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 나 자신이 아니라면 입력 무시
        if (false == photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
            playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, remoteCamRot, 10 * Time.deltaTime);
            return;
        }

        // 마우스 입력에 따라 카메라는 상하로만 회전
        RotateplayerCamera();
        // 좌우는 몸체 회전하고 싶다
        RotateBody();
        // 사용자 입력에 따라 이동하고 싶다
        Move();
    }

    private void RotateplayerCamera()
    {
        float value = Input.GetAxis("Mouse Y");
        camAngle += value * rotSpeed * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -60, 60);

        // playerCamera는 플레이어의 자식이므로 로컬 기준에서 돌려야 함 
        playerCamera.localEulerAngles = new Vector3(-camAngle, 0, 0);

    }

    private void RotateBody()
    {
        float value = Input.GetAxis("Mouse X");
        bodyAngle += value * rotSpeed * Time.deltaTime;

        // 좌우는 플레이어 전체를 회전 
        transform.eulerAngles = new Vector3(0, bodyAngle, 0);
    }

    private void Move()
    {
        // 1. 사용자 입력에 따라 
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // 2. 방향이 필요
        Vector3 dir = new Vector3(h, 0, v) * speed; // speed를 곱해서 벡터의 길이까지
        // -> 카메라가 바라보는 방향으로 방향 전환 
        dir = playerCamera.TransformDirection(dir);

        // 바닥에 있으면 수직 속도를 0으로 하자 (수직항력)
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }
        // 점프
        if (Input.GetButtonDown("Jump"))
        {
            yVelocity = jumpPower;
        }
        // 중력을 적용하고 싶다. v = v0 + at
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        // 3. 이동하고 싶다 p = p0 + vt
        cc.Move(dir * Time.deltaTime);
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
            stream.SendNext(playerCamera.rotation);
        }
        // 내가 데이터를 받는 중이라면 
        else // 원격에 있는 나 
        {
            // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
            // - 여기서 바로 적용하면 뚝뚝 끊김
            //transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            //playerCamera.rotation = (Quaternion)stream.ReceiveNext(); 
            remotePos = (Vector3)stream.ReceiveNext();
            remoteRot = (Quaternion)stream.ReceiveNext();
            remoteCamRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

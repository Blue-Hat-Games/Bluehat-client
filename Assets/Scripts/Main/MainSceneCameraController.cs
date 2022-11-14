using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainSceneCameraController : MonoBehaviour
{
    public GameObject target;       // 타겟이 될 게임오브젝트
    private Vector3 point = Vector3.zero;   // 타겟의 위치(바라볼 위치)

    private float rotationX = 0.0f;         // X축 회전값
    private float rotationY = 0.0f;         // Y축 회전값
    private float speed = 100.0f;           // 회전속도

    void Start()
    {
        // 바라볼 위치 얻기
        point = target.transform.position;
        // 마우스 변화량을 얻고, 그 값에 델타타임과 속도를 곱해서 회전값 구하기
        rotationX = Input.GetAxis("Mouse X") * Time.deltaTime * speed;
        rotationY = Input.GetAxis("Mouse Y") * Time.deltaTime * speed;

        // 각 축으로 회전
        // Y축은 마우스를 내릴때 카메라는 올라가야 하므로 반대로 적용
        transform.RotateAround(point, Vector3.right, -rotationY);
        transform.RotateAround(point, Vector3.up, rotationX);

        // 회전후 타겟 바라보기
        transform.LookAt(point);

    }

    public float maxUp;
    public float maxDown;
    void Update()
    {
        // 마우스가 눌러지면,
        if (Input.GetMouseButton(0))
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // 회전후 타겟 바라보기
            transform.LookAt(point);

            // 마우스 변화량을 얻고, 그 값에 델타타임과 속도를 곱해서 회전값 구하기
            var horizontalAxis = Input.GetAxis("Mouse X") * Time.deltaTime * speed;
            var verticalAxis = Input.GetAxis("Mouse Y") * Time.deltaTime * speed;

            if(rotationY + verticalAxis > maxUp)
            {
                verticalAxis = maxUp - rotationY;
            }
            else if(rotationY + verticalAxis < maxDown)
            {
                verticalAxis = maxDown - rotationY;
            }

            // 각 축으로 회전
            // Y축은 마우스를 내릴때 카메라는 올라가야 하므로 반대로 적용
            transform.RotateAround(point, Vector3.right, verticalAxis);
            transform.RotateAround(point, Vector3.up, horizontalAxis);

            rotationY += verticalAxis;

        }
    }
}

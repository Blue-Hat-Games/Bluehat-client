using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MultiplayCameraController : MonoBehaviour
{
    private GameObject player;
    public float speed;

    private int rightFingerId;
    float halfScreenWidth; //화면 절반만 터치하면 카메라 회전
    private Vector2 prevPoint;
    private Vector3 originPos;
    public Button btn; // 시점(yaw)을 원상태로 되돌리는 버튼

    public Transform cameraTransform;

    private Vector2 lookInput;
    private float cameraPitch; // pitch 시점

    private float distanceY = 7;
    private float distanceZ = -15;

    private Camera cameraComp;

    public void SetCameraTarget(GameObject animal)
    {
        // this.transform.SetParent(animal.transform, false);
        this.transform.position = new Vector3(animal.transform.position.x, animal.transform.position.y + distanceY, animal.transform.position.z + distanceZ);
        player = animal;

        Debug.Log($"target setting completed => {animal.name}");
    }

    public void AdjustCamDistance(float addValue)
    {
        distanceZ -= addValue;
        distanceY += (addValue * 1.3f);
    }

    Vector3 FirstPoint;
    Vector3 SecondPoint;
    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;
    
    void Start()
    {
        xAngle = 0;
        yAngle = 0;
        this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0);
        cameraComp = this.gameObject.GetComponentInChildren<Camera>();
        
        this.transform.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + distanceY, player.transform.position.z - distanceZ);

        rightFingerId = -1; // -1은 추적중이 아닌 손가락
        halfScreenWidth = Screen.width / 2;
        originPos = Vector3.zero;
        cameraPitch = 35f;

        if(btn == null)
        {
            return;
        }
        this.btn.onClick.AddListener(() => {
            this.transform.eulerAngles = this.originPos;
        });
    }

    void Update()
    {
        if(player == null)
        {
            return;
        }
        Vector3 goalPos = new Vector3(player.transform.position.x, player.transform.position.y + distanceY, player.transform.position.z - distanceZ);
        this.transform.position = Vector3.Lerp(this.transform.position, goalPos, this.speed);
        cameraComp.transform.LookAt(player.transform.position);                


        if(Application.isEditor)
        {
            float rotX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

            if(rotX != 0 && rotY != 0)
            {
                cameraComp.transform.RotateAround(player.transform.position, Vector3.right, -rotY);
                cameraComp.transform.RotateAround(player.transform.position, Vector3.up, -rotX);

                Vector3 localAngle = cameraComp.transform.localEulerAngles;
                cameraComp.transform.LookAt(player.transform.position);
            }
        }
        
        GetTouchInput();
    }

    public Vector3 GetCameraForwardVector()
    {
        return this.cameraComp.transform.forward;
    }

    public Transform GetCameraTransform()
    {
        if(cameraComp == null)
        {
            cameraComp = this.gameObject.GetComponentInChildren<Camera>();
        }
        return this.cameraComp.transform;
    }
    
    Vector2 touchDeltaPosition;
    
    public Vector2 GetTouchDeltaPosition()
    {
        return touchDeltaPosition;
    }

    private void GetTouchInput()
    {
        for(int i=0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            switch(t.phase)
            {
                case TouchPhase.Began:
                    
                    if(t.position.x > this.halfScreenWidth && this.rightFingerId == -1)
                    {
                        this.rightFingerId = t.fingerId;
                        FirstPoint = t.position;
                        xAngleTemp = xAngle;
                        yAngleTemp = yAngle;
                    }
                    break;
                case TouchPhase.Moved:

                    if(t.position.x > this.halfScreenWidth && t.fingerId == this.rightFingerId)
                    {
                        // 수평
                        this.prevPoint = t.position - t.deltaPosition;
                        touchDeltaPosition = t.deltaPosition;

                        Vector3 adjustRotatePoint = new Vector3(this.player.transform.position.x, this.player.transform.position.y + 5, this.player.transform.position.z);
                        cameraComp.transform.RotateAround(adjustRotatePoint, Vector3.up, -(t.position.x - this.prevPoint.x) * 0.2f);
                        cameraComp.transform.RotateAround(adjustRotatePoint, Vector3.right, -(t.position.y - this.prevPoint.y) * 0.2f);
                        this.prevPoint = t.position;
                        
                        cameraComp.transform.LookAt(player.transform.position);                
                    }
                    
                    break;
                case TouchPhase.Stationary:
                    if(t.fingerId == this.rightFingerId)
                    {
                        this.lookInput = Vector2.zero;
                    }
                    break;
                case TouchPhase.Ended:
                    if(t.fingerId == this.rightFingerId)
                    {
                        this.rightFingerId = -1;
                        touchDeltaPosition = Vector2.zero;
                        Debug.Log("오른쪽 손가락 끝");
                    }
                    break;
                case TouchPhase.Canceled:
                    if(t.fingerId == this.rightFingerId)
                    {
                        this.rightFingerId = -1;
                        touchDeltaPosition = Vector2.zero;
                        Debug.Log("오른쪽 손가락 끝");
                    }
                    break;
            }
        }
    }
}

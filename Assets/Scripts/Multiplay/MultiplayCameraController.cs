using UnityEngine;
using UnityEngine.UI;

public class MultiplayCameraController : MonoBehaviour
{
    public float speed;
    public Button btn; // 시점(yaw)을 원상태로 되돌리는 버튼

    public Transform cameraTransform;

    private Camera cameraComp;
    private float cameraPitch; // pitch 시점

    private float distanceY = 7;
    private float distanceZ = -15;

    private Vector3 FirstPoint;
    private float halfScreenWidth; //화면 절반만 터치하면 카메라 회전

    private Vector2 lookInput;
    private Vector3 originPos;
    private GameObject player;
    private Vector2 prevPoint;

    private int rightFingerId;
    private Vector3 SecondPoint;

    private Vector2 touchDeltaPosition;
    private float xAngle;
    private float xAngleTemp;
    private float yAngle;
    private float yAngleTemp;

    private void Start()
    {
        xAngle = 0;
        yAngle = 0;
        transform.rotation = Quaternion.Euler(yAngle, xAngle, 0);
        cameraComp = gameObject.GetComponentInChildren<Camera>();

        transform.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + distanceY,
            player.transform.position.z - distanceZ);

        rightFingerId = -1; // -1은 추적중이 아닌 손가락
        halfScreenWidth = Screen.width / 2;
        originPos = Vector3.zero;
        cameraPitch = 35f;

        if (btn == null) return;
        btn.onClick.AddListener(() => { transform.eulerAngles = originPos; });
    }

    private void Update()
    {
        if (player == null) return;
        var goalPos = new Vector3(player.transform.position.x, player.transform.position.y + distanceY,
            player.transform.position.z - distanceZ);
        transform.position = Vector3.Lerp(transform.position, goalPos, speed);
        cameraComp.transform.LookAt(player.transform.position);


        if (Application.isEditor)
        {
            var rotX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
            var rotY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;

            if (rotX != 0 && rotY != 0)
            {
                cameraComp.transform.RotateAround(player.transform.position, Vector3.right, -rotY);
                cameraComp.transform.RotateAround(player.transform.position, Vector3.up, -rotX);

                var localAngle = cameraComp.transform.localEulerAngles;
                cameraComp.transform.LookAt(player.transform.position);
            }
        }

        GetTouchInput();
    }

    public void SetCameraTarget(GameObject animal)
    {
        // this.transform.SetParent(animal.transform, false);
        transform.position = new Vector3(animal.transform.position.x, animal.transform.position.y + distanceY,
            animal.transform.position.z + distanceZ);
        player = animal;

        Debug.Log($"target setting completed => {animal.name}");
    }

    public void AdjustCamDistance(float addValue)
    {
        distanceZ -= addValue;
        distanceY += addValue * 1.3f;
    }

    public Vector3 GetCameraForwardVector()
    {
        return cameraComp.transform.forward;
    }

    public Transform GetCameraTransform()
    {
        if (cameraComp == null) cameraComp = gameObject.GetComponentInChildren<Camera>();
        return cameraComp.transform;
    }

    public Vector2 GetTouchDeltaPosition()
    {
        return touchDeltaPosition;
    }

    private void GetTouchInput()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            var t = Input.GetTouch(i);
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        rightFingerId = t.fingerId;
                        FirstPoint = t.position;
                        xAngleTemp = xAngle;
                        yAngleTemp = yAngle;
                    }

                    break;
                case TouchPhase.Moved:

                    if (t.position.x > halfScreenWidth && t.fingerId == rightFingerId)
                    {
                        // 수평
                        prevPoint = t.position - t.deltaPosition;
                        touchDeltaPosition = t.deltaPosition;

                        var adjustRotatePoint = new Vector3(player.transform.position.x,
                            player.transform.position.y + 5, player.transform.position.z);
                        cameraComp.transform.RotateAround(adjustRotatePoint, Vector3.up,
                            -(t.position.x - prevPoint.x) * 0.2f);
                        cameraComp.transform.RotateAround(adjustRotatePoint, Vector3.right,
                            -(t.position.y - prevPoint.y) * 0.2f);
                        prevPoint = t.position;

                        cameraComp.transform.LookAt(player.transform.position);
                    }

                    break;
                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId) lookInput = Vector2.zero;
                    break;
                case TouchPhase.Ended:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        touchDeltaPosition = Vector2.zero;
                        Debug.Log("오른쪽 손가락 끝");
                    }

                    break;
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        touchDeltaPosition = Vector2.zero;
                        Debug.Log("오른쪽 손가락 끝");
                    }

                    break;
            }
        }
    }
}
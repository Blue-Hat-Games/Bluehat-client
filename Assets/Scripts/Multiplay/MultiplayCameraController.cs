using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiplayCameraController : MonoBehaviour
{
    public Button btn; // 시점(yaw)을 원상태로 되돌리는 버튼

    public Transform cameraTransform;
    public float cameraSensitivity;
    private float cameraPitch; // pitch 시점
    private float halfScreenWidth; //화면 절반만 터치하면 카메라 회전

    private Vector2 lookInput;
    private Vector3 originPos;
    private GameObject player;

    private Vector2 prevPoint;
    // public float speed;

    private int rightFingerId;

    private void Start()
    {
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
        // this.transform.position = Vector3.Lerp(this.transform.position, this.player.transform.position + new Vector3(0, this.transform.position.y, 0), this.speed);

        // GetTouchInput();
    }

    public void SetCameraTarget(GameObject animal)
    {
        transform.SetParent(animal.transform, false);
        transform.localPosition = new Vector3(0, 4, -10);
        player = animal;
        Debug.Log($"target setting completed => {animal.name}");
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
                        Debug.Log("오른쪽 손가락 입력");
                    }

                    break;
                case TouchPhase.Moved:
                    // 시점 원상태 버튼을 누를 때 화면이 돌아가도록 하지 않기 위해 
                    if (!EventSystem.current.IsPointerOverGameObject(i))
                        if (t.fingerId == rightFingerId)
                        {
                            // 수평
                            prevPoint = t.position - t.deltaPosition;
                            transform.RotateAround(player.transform.position, Vector3.up,
                                -(t.position.x - prevPoint.x) * 0.2f);
                            prevPoint = t.position;

                            // 수직
                            lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                            cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, 10f, 35f);
                            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
                        }

                    break;
                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId) lookInput = Vector2.zero;
                    break;
                case TouchPhase.Ended:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        Debug.Log("오른쪽 손가락 끝");
                    }

                    break;
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        Debug.Log("오른쪽 손가락 끝");
                    }

                    break;
            }
        }
    }
}
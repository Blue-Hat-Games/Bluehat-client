using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float cameraRotSpeed = 3;
    private float deltaX;
    private Joystick joystick;
    private Transform targetTr;

    private void Start()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (null == targetTr || null == joystick)
            return;


        var lookAxisRot = Quaternion.LookRotation(targetTr.forward);
        var projRot = Vector3.ProjectOnPlane(lookAxisRot.eulerAngles, Vector3.right);

        transform.position = new Vector3(targetTr.position.x, targetTr.position.y + 3, targetTr.position.z - 10);
    }

    public void SetCameraTarget(GameObject animal)
    {
        targetTr = animal.transform;

        Debug.Log($"target setting completed => {animal.name}");
    }
}
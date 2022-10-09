using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float cameraRotSpeed = 3;
    private Transform targetTr;
    private float deltaX;
    private Joystick joystick;

    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    public void SetCameraTarget(GameObject animal)
    {
        targetTr = animal.transform;

        Debug.Log($"target setting completed => {animal.name}");
    }

    // Update is called once per frame
    void Update()
    {
        if (null == targetTr || null == joystick)
            return;


        var lookAxisRot = Quaternion.LookRotation(targetTr.forward);
        var projRot = Vector3.ProjectOnPlane(lookAxisRot.eulerAngles, Vector3.right);

        // this.transform.rotation = Quaternion.Euler(projRot);
        //this.transform.RotateAround(targetTr.position, Vector3.up, joystick.Horizontal * Time.deltaTime * this.cameraRotSpeed);
        this.transform.position = new Vector3(targetTr.position.x, targetTr.position.y + 3, targetTr.position.z - 10);
    }
}

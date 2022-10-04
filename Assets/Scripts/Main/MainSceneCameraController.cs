using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCameraController : MonoBehaviour
{
    public GameObject target;       // Ÿ���� �� ���ӿ�����Ʈ
    private Vector3 point = Vector3.zero;   // Ÿ���� ��ġ(�ٶ� ��ġ)
 
    private float rotationX = 0.0f;         // X�� ȸ����
    private float rotationY = 0.0f;         // Y�� ȸ����
    private float speed = 100.0f;           // ȸ���ӵ�
 
    void Start()
    {
        // �ٶ� ��ġ ���
        point = target.transform.position;
        // ���콺 ��ȭ���� ���, �� ���� ��ŸŸ�Ӱ� �ӵ��� ���ؼ� ȸ���� ���ϱ�
            rotationX = Input.GetAxis("Mouse X") * Time.deltaTime * speed;
            rotationY = Input.GetAxis("Mouse Y") * Time.deltaTime * speed;
 
            // �� ������ ȸ��
            // Y���� ���콺�� ������ ī�޶�� �ö󰡾� �ϹǷ� �ݴ�� ����
            transform.RotateAround(point, Vector3.right, -rotationY);
            transform.RotateAround(point, Vector3.up, rotationX);
 
            // ȸ���� Ÿ�� �ٶ󺸱�
            transform.LookAt(point);
            
    }
 
    void Update()
    {
        // ���콺�� ��������,
        if (Input.GetMouseButton(0))
        {
            // ���콺 ��ȭ���� ���, �� ���� ��ŸŸ�Ӱ� �ӵ��� ���ؼ� ȸ���� ���ϱ�
            rotationX = Input.GetAxis("Mouse X") * Time.deltaTime * speed;
            rotationY = Input.GetAxis("Mouse Y") * Time.deltaTime * speed;
 
            // �� ������ ȸ��
            // Y���� ���콺�� ������ ī�޶�� �ö󰡾� �ϹǷ� �ݴ�� ����
            transform.RotateAround(point, Vector3.right, -rotationY);
            transform.RotateAround(point, Vector3.up, rotationX);
 
            // ȸ���� Ÿ�� �ٶ󺸱�
            transform.LookAt(point);
        }
    }
}

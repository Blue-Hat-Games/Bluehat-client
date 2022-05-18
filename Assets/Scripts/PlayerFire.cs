using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ڰ� �߻� ��ư�� ������ ���� �߻��ϰ� �ʹ�.
public class PlayerFire : MonoBehaviour
{
    Camera cam;
    void Start()
    {
        // ���� �ִ� �κ�?�� ���� ī�޶� ��������
        Camera.main.gameObject.SetActive(false);
        // ���� �÷��̾ �ڽ��̶�� layer�� Player�� 
        // �׷��� ������ layer�� enemy�� ��������

        // ���� ������ �ٸ� Ŭ���̾�Ʈ�� ī�޶� ���� ���� Transform�� �ƴ� Camera�� ������
        cam = GetComponentInChildren<Camera>();
        cam.tag = "MainCamera";
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hitInfo;
            int layerMask = 1 << gameObject.layer; 
            if(Physics.Raycast(ray, out hitInfo, 1000, ~layerMask)) // �÷��̾ ����
            {
                // ������!
                //Destroy(hitInfo.transform.gameObject);

            }

        }
        
    }
}

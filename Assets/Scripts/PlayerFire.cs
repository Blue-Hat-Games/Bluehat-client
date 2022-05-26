using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ����ڰ� �߻� ��ư�� ������ ���� �߻��ϰ� �ʹ�.
public class PlayerFire : MonoBehaviourPun // Pun ����ؼ� Network ��ü�� �� 
{
    Camera cam;
    void Start()
    {
        // CHECK: Network Manager���� �������� �� ���� ���� ����!

        // �� �ڽ��� ��쿡�� ó���ǵ��� ���� 
        if (photonView.IsMine)
        {
            // ���� �ִ� �κ�?�� ���� ī�޶� ��������
            Camera.main.gameObject.SetActive(false);
            // ���� ������ �ٸ� Ŭ���̾�Ʈ�� ī�޶� ���� ���� Transform�� �ƴ� Camera�� ������
            cam = GetComponentInChildren<Camera>();
            cam.tag = "MainCamera";
            // ���� �÷��̾ �ڽ��̶�� layer�� Player�� 
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.name = "Player";
        }
        else
        {
            // �׷��� ������ layer�� enemy�� ��������
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            cam = GetComponentInChildren<Camera>();
            // enemy�� ī�޶� Ȱ��ȭ�Ǿ����� �ʿ䰡 ���� ������ 
            cam.enabled = false;
            gameObject.name = "Enemy";
        }           
    }

    void Update()
    {
        // �� �ڽ��� �ƴ϶�� �Է� ����
        if(photonView.IsMine == false)
        {
            return;
        }
           
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

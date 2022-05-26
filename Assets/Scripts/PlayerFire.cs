using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 사용자가 발사 버튼을 누르면 총을 발사하고 싶다.
public class PlayerFire : MonoBehaviourPun // Pun 상속해서 Network 객체가 됨 
{
    Camera cam;
    void Start()
    {
        // CHECK: Network Manager에서 꺼버리는 게 나을 수도 있음!

        // 나 자신일 경우에만 처리되도록 하자 
        if (photonView.IsMine)
        {
            // 원래 있던 로비?용 메인 카메라 꺼버리기
            Camera.main.gameObject.SetActive(false);
            // 나를 제외한 다른 클라이언트의 카메라를 끄기 위해 Transform이 아닌 Camera를 가져옴
            cam = GetComponentInChildren<Camera>();
            cam.tag = "MainCamera";
            // 현재 플레이어가 자신이라면 layer를 Player로 
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.name = "Player";
        }
        else
        {
            // 그렇지 않으면 layer를 enemy로 지정하자
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            cam = GetComponentInChildren<Camera>();
            // enemy의 카메라가 활성화되어있을 필요가 없기 때문에 
            cam.enabled = false;
            gameObject.name = "Enemy";
        }           
    }

    void Update()
    {
        // 나 자신이 아니라면 입력 무시
        if(photonView.IsMine == false)
        {
            return;
        }
           
        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hitInfo;
            int layerMask = 1 << gameObject.layer; 
            if(Physics.Raycast(ray, out hitInfo, 1000, ~layerMask)) // 플레이어만 빼고
            {
                // 없애자!
                //Destroy(hitInfo.transform.gameObject);

            }

        }
        
    }
}

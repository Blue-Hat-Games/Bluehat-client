using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자가 발사 버튼을 누르면 총을 발사하고 싶다.
public class PlayerFire : MonoBehaviour
{
    Camera cam;
    void Start()
    {
        // 원래 있던 로비?용 메인 카메라 꺼버리기
        Camera.main.gameObject.SetActive(false);
        // 현재 플레이어가 자신이라면 layer를 Player로 
        // 그렇지 않으면 layer를 enemy로 지정하자

        // 나를 제외한 다른 클라이언트의 카메라를 끄기 위해 Transform이 아닌 Camera를 가져옴
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
            if(Physics.Raycast(ray, out hitInfo, 1000, ~layerMask)) // 플레이어만 빼고
            {
                // 없애자!
                //Destroy(hitInfo.transform.gameObject);

            }

        }
        
    }
}

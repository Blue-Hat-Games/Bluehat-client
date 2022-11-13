using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Obstacle : MonoBehaviour
{
    private PhotonView pv;
    public int obstacleRespawnTime = 15;

    void Start()
    {
        pv = this.gameObject.GetComponent<PhotonView>();
    }


   [PunRPC] // 이 밑의 함수는 RPC함수가 되고, 원격에서 호출할 수 있는 상태가 됨
    public void ShowOffObstacle() 
    {
        StartCoroutine(ReactiveObstacle()); 
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer ren in renderers)
        {
            ren.enabled = false;
        }
        // this.gameObject.SetActive(false);
    }

    IEnumerator ReactiveObstacle()
    {
        yield return new WaitForSeconds(obstacleRespawnTime);
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer ren in renderers)
        {
            ren.enabled = true;
        }
        // this.gameObject.SetActive(true);
    }
}

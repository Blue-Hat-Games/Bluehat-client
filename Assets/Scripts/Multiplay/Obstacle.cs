using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int obstacleRespawnTime = 15;
    private PhotonView pv;

    private void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
    }


    [PunRPC] // 이 밑의 함수는 RPC함수가 되고, 원격에서 호출할 수 있는 상태가 됨
    public void ShowOffObstacle()
    {
        StartCoroutine(ReactiveObstacle());
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var ren in renderers) ren.enabled = false;
        // this.gameObject.SetActive(false);
    }

    private IEnumerator ReactiveObstacle()
    {
        yield return new WaitForSeconds(obstacleRespawnTime);
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var ren in renderers) ren.enabled = true;
        // this.gameObject.SetActive(true);
    }
}
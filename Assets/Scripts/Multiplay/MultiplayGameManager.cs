using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class MultiplayGameManager : MonoBehaviour
{
    public static MultiplayGameManager instance = null;
    private bool isConnect = false;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }
    void Start()
    {
        StartCoroutine(CreatePlayer());
    }

    public bool IsConnectTrue() {
        Debug.Log("IsConnectTrue");
        return isConnect;
    }

    public void SetIsConnectTrue() {
        Debug.Log("SetIsConnectTrue");
        this.isConnect = true;
    }
    
    IEnumerator CreatePlayer()
    {
        Debug.Log("MultiplayGameManager => CreatePlayer()");
        // isConnect = true
        // yield return new WaitUntil(() => isConnect);
        // TODO: 로딩이 다 되고나서 되어야 하는데, 관련 포톤 함수를 못찾아서 일단 3초 정도 지연 후 Create 되도록 함
        yield return new WaitForSeconds(3); // TEST 
        // 360도 Sphere 공간안에서 랜덤으로 한 점을 찍은 것
        Vector3 randPos = Random.insideUnitSphere * 10;
        // 0,0에서 10m 사이 까지의 거리 중 랜덤으로 설정
        randPos.y = 0;
        // 클라이언트가 새로 방에 들어오면 마스터 클라이언트가 자동으로 환경을 맞춰줌 
        GameObject playerTemp = PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }
}

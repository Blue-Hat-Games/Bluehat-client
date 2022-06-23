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
        // TODO: �ε��� �� �ǰ��� �Ǿ�� �ϴµ�, ���� ���� �Լ��� ��ã�Ƽ� �ϴ� 3�� ���� ���� �� Create �ǵ��� ��
        yield return new WaitForSeconds(3); // TEST 
        // 360�� Sphere �����ȿ��� �������� �� ���� ���� ��
        Vector3 randPos = Random.insideUnitSphere * 10;
        // 0,0���� 10m ���� ������ �Ÿ� �� �������� ����
        randPos.y = 0;
        // Ŭ���̾�Ʈ�� ���� �濡 ������ ������ Ŭ���̾�Ʈ�� �ڵ����� ȯ���� ������ 
        GameObject playerTemp = PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }
}

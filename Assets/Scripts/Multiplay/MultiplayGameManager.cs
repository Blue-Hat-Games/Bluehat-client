using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class MultiplayGameManager : MonoBehaviour
{
    public static MultiplayGameManager instance = null;
    public bool isConnect = false;
    
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

    IEnumerator CreatePlayer()
    {
        yield return new WaitUntil(() => isConnect);
        // 360�� Sphere �����ȿ��� �������� �� ���� ���� ��
        Vector3 randPos = Random.insideUnitSphere * 10;
        // 0,0���� 10m ���� ������ �Ÿ� �� �������� ����
        randPos.y = 0;
        // Ŭ���̾�Ʈ�� ���� �濡 ������ ������ Ŭ���̾�Ʈ�� �ڵ����� ȯ���� ������ 
        GameObject playerTemp = PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }
}

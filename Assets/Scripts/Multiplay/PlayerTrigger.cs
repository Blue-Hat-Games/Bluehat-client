using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ����ڰ� Obstacle�̶� �ε����� �ش� Obstacle�� ��������� �ϰ� �ʹ�.
// Obstacle�� PhotonView�� �ִ� ��ü���� ��
namespace BluehatGames {
public class PlayerTrigger : MonoBehaviourPun
{
    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Obstacle") {
            // ���׸� �������� �����ش�.
            PlayerStatusController.instance.AddAetherEnergy();
            PhotonView pv = coll.gameObject.GetComponent<PhotonView>();
            if(pv.IsMine)
            {
                PhotonNetwork.Destroy(coll.gameObject);
            }
        }
    }
}
}
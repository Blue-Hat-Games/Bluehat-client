using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 사용자가 Obstacle이랑 부딪히면 해당 Obstacle이 사라지도록 하고 싶다.
// Obstacle은 PhotonView가 있는 객체여야 함
namespace BluehatGames
{
    public class PlayerTrigger : MonoBehaviourPun
    {
        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "Obstacle")
            {
                // 에테르 게이지를 더해준다.
                PlayerStatusController.instance.AddAetherEnergy();
                PhotonView pv = coll.gameObject.GetComponent<PhotonView>();
                if (pv.IsMine)
                {
                    PhotonNetwork.Destroy(coll.gameObject);
                }
            }
        }
    }
}
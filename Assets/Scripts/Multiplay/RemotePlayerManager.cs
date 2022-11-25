using System;
using Photon.Pun;
using UnityEngine;

namespace BluehatGames
{
    // 다른 클라이언트 플레이어들의 텍스처, 모자 등을 바꿔주는 스크립트
    public class RemotePlayerManager : MonoBehaviourPun, IPunObservable
    {
        private SelectedAnimalDataCupid cupid;
        private AnimalDataFormat data;
        private bool isCompleted;
        private MultiplayGameManager multiplayGameManager;

        private int remoteId;

        private void Start()
        {
            cupid = FindObjectOfType<SelectedAnimalDataCupid>();
            multiplayGameManager = FindObjectOfType<MultiplayGameManager>();
        }

        private void Update()
        {
        }

        // 내가 들어왔을 때 다른 클라이언트에게 정보를 보냄
        // 다른 클라이언트가 들어왔을 때 내가 정보를 얻어야 함 
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (isCompleted == false)
                {
                    var myViewID = MultiplayGameManager.instance.GetMyPlayerPhotonViewID();
                    Debug.Log($"Sender: MyViewID = {myViewID}");
                    stream.SendNext(myViewID);
                    stream.SendNext(cupid.GetSelectedAnimalData());
                }
            }
            else
            {
                remoteId = (int)stream.ReceiveNext();
                Debug.Log($"Receiver : remoteId => {remoteId}");
                data = (AnimalDataFormat)stream.ReceiveNext();
                Debug.Log($"Receiver : hat_item => {data.headItem}");

                var remoteAnimalObject = PhotonNetwork.GetPhotonView(remoteId).gameObject;
                cupid.SetAnimalTexture(remoteAnimalObject);
                cupid.LoadHatItemPrefab(data.headItem, remoteAnimalObject);
                isCompleted = true;
            }
        }

        [Serializable]
        public class RemoteClientData
        {
            private AnimalDataFormat animalDataFormat;
            private int myPhotonViewId;

            private RemoteClientData(int viewId, AnimalDataFormat dataForm)
            {
                myPhotonViewId = viewId;
                animalDataFormat = dataForm;
            }
        }
    }
}
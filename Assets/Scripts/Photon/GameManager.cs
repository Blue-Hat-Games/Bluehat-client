using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {


        #region Photon Callbacks

        // 로컬 플레이어가 룸을 떠날 때 호출되고, 'Launcher' 씬을 로드함 
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        #endregion


        #region Public Methods
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public static GameManager Instance;

        /* 이 메소드는 명시적으로 플레이어를 Photon Network 룸에서 나가도록 하며 
         * 추상화를 위해서 public 메소드로 wrap 하였습니다. 
         * 다음 단계에서는 데이터 저장 또는 확인 단계 추가등 더 많은 기능 추가를 원할 것 입니다.
         * */

        // 'Leave Button' 온클릭 메서드
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region MonoBehaviour CallBacks
        void Start()
        {
            Instance = this;
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                // 로컬 플레이어 인스턴스가 없으면,
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    // 
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }

            }
        }
        #endregion

        #region Private Methods


        void LoadArena()
        {
            /* PhotonNetwork.LoadLevel() 은 마스터 클라이언트인 경우에만 호출 되어야 합니다. 
             * 따라서 PhotonNetwork.isMasterClient 를 이용하여 마스터 클라이어인트인지를 체크 합니다. 
             * 이것을 체크하는 것은 호출자의 책임이 될 것 이며 이 섹션의 다음 파트에서 다룰 것 입니다.
             * PhotonNetwork.LoadLevel() 을 이용하여 원하는 레벨을 로드 합니다. 
             * 이 게임에서는 PhotonNetwork.automaticallySyncScene 을 사용하도록 해놓았기 때문에 
             * 룸 안의 모든 접속한 클라이언트에 대해 이 레벨 로드를 유니티가 직접 하는 것이 아닌 Photon 이 하도록 하였습니다.
            */
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }


        #endregion

        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            // PhotonNetwork.isMasterClient 를 이용하여 마스터인 경우에만 LoadArena() 를 호출 할 것 입니다
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion
    }
}
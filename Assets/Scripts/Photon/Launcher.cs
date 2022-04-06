using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        // 1. 룸 당 최대 인원 수 설정
        // 2. 가득 찬 경우 새로운 룸을 만들게 됨 
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion


        #region Private Fields

        // 1. 클라이언트의 버전 번호
        // 2. 유저들이 게임 버전에 따라 나누어서 입장하게 됨 
        [Tooltip("This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).")]
        string gameVersion = "1";

        // 1. 현재 프로세스를 추적함
        // 2. 연결은 비동기식이며 Photon의 여러 콜백을 기반으로 함
        // 3. Photon에서 콜백을 받을 때 동작을 적절하게 조정하려면 이것을 추적해야 함
        // 4. 일반적으로 OnConnectedToMaster() 콜백에 사용됨
        bool isConnecting;

        #endregion


        #region Public Fields

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion


        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // 존재하는 룸에 우선 조인을 시도하고, 없다면 OnJoinRandomFailed() 가 호출됨
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        // JoinRandomRoom()에 실패하면 호출됨
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // 만약 랜덤 룸 입장에 실패하면, 룸이 없거나 룸이 가득찬 경우라서 이럴 땐 새로운 룸을 생성함
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        // 클라이언트가 룸에 입장했을 때 호출됨
        public override void OnJoinedRoom()
        {
            // 만약에 첫번째 플레이어라면 'Room for 1' 씬을 로드함
            // - we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }

            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // 이것은 마스터 클라이언트에서 PhotonNetwork.LoadLevel()을 사용할 수 있고,
            // 같은 방에 있는 모든 클라이언트가 자동으로 레벨을 동기화할 수 있는지 확인합니다.
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }
        #endregion


        #region Public Methods

        // 1. connection 과정 시작
        // - 이미 연결되었다면, 랜덤룸 입장 시도
        // - 연결되지 않았다면, Photon cloud network에 연결을 시도
        public void Connect()
        {
            // 참여하려는 의사를 계속 추적. 게임에서 돌아올 때 연결되었다는 콜백을 받을 것이기 때문에 어떻게 해야 하는지 알아야 하기 때문
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // 연결되었는지 아닌지 확인해서 조인하거나 서버에 연결을 시도
            if (PhotonNetwork.IsConnected)
            {
                // 랜덤룸 조인 시도
                // - 실패하면 OnJoinRandomFailed()가 호출되고 하나의 방을 더 만듦        
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 가장 먼저 Photon Online Server에 연결해야 함
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        #endregion
    }
}
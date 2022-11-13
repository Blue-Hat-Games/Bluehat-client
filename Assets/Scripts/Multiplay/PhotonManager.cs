using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


namespace BluehatGames
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        private readonly string gameVersion = "v1.0";
        private byte maxPlayersPerRoom = 8;
        private bool isConnecting = false;

        [Header("UI")]
        // public InputField nickNameInputField;
        public Button connectButton;
        public Button goToMainButton;

        public GameObject controlPanel; // inputField, PlayButton
        public GameObject progressPanel; // connecting ...
        void Awake()
        {
            // 이후에 들어온 플레이어에게 현재 씬 상황을 자동으로 적용시켜줌
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        void Start()
        {
            controlPanel.SetActive(true);
            progressPanel.SetActive(false);
            Debug.Log("00. 포톤 매니저 시작");

            // Connect 버튼을 누른 경우 접속 시도 
            // - 닉네임 설정은 PlayerNameInputField에서 해줌
            connectButton.onClick.AddListener(() =>
            {
                Connect();
            });
            connectButton.gameObject.SetActive(false);

            goToMainButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });
        }

        public void SetconnectButtonActive(bool isActive)
        {
            connectButton.gameObject.SetActive(isActive);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("01. 포톤 서버에 접속");
            if (isConnecting)
            {
                // 존재하는 룸에 우선 조인을 시도하고, 없다면 OnJoinRandomFailed() 가 호출됨
                PhotonNetwork.JoinRandomRoom();
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        // JoinRandomRoom()에 실패하면 호출됨
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("02. 랜덤 룸 접속 실패");

            // 룸 속성 설정
            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = true;
            ro.MaxPlayers = maxPlayersPerRoom; // 8명까지만 입장가능하게 하자 

            // 만약 랜덤 룸 입장에 실패하면, 룸이 없거나 룸이 가득찬 경우라서 이럴 땐 새로운 룸을 생성함
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("03. 방 생성 완료");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("04. 방 입장 완료");
            if (PhotonNetwork.IsMasterClient)
            {
                // 방 입장을 위한 씬 로드
                PhotonNetwork.LoadLevel(SceneName._PhotonNetworkScene);
                StartCoroutine(RepeatIsConnect());
            }
        }

        IEnumerator RepeatIsConnect()
        {
            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                Debug.Log($"PhotonNetwork.LevelLoadingProgress => {PhotonNetwork.LevelLoadingProgress}");
                yield return new WaitForEndOfFrame();
            }

            MultiplayGameManager.instance?.SetIsConnectTrue();
            // while(true) {
            //     yield return null;
            //     Debug.Log("RepeatIsConnect....");
            //     if(MultiplayGameManager.instance) {
            //         MultiplayGameManager.instance?.SetIsConnectTrue();
            //         if(MultiplayGameManager.instance.IsConnectTrue()) {
            //             yield break;
            //         }    
            //     }


            // }
        }

        // 1. connection 과정 시작
        // - 이미 연결되었다면, 랜덤룸 입장 시도
        // - 연결되지 않았다면, Photon cloud network에 연결을 시도
        public void Connect()
        {
            Debug.Log("------------------------ Connect ------------------");
            // 참여하려는 의사를 계속 추적. 게임에서 돌아올 때 연결되었다는 콜백을 받을 것이기 때문에 어떻게 해야 하는지 알아야 하기 때문
            isConnecting = true;

            controlPanel.SetActive(false);
            progressPanel.SetActive(true);

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

    }
}
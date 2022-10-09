using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine;
using UnityEngine.UI;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전

    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 룸 접속 버튼

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start()
    {

        //접속에 필요한 정보 (게임 버전) 설정
        PhotonNetwork.GameVersion = this.gameVersion;
        //설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();


        this.joinButton.interactable = false;
        this.connectionInfoText.text = "마스터 서버에 접속중...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        this.joinButton.interactable = true;
        this.connectionInfoText.text = "온라인 : 마스터 서버와 연결 됨";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        this.joinButton.interactable = false;
        this.connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도중... ";
        //설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        // 중복 접속 막기
        this.joinButton.interactable = false;

        // 마스터 서버에 접속 중이라면
        if (PhotonNetwork.IsConnected)
        {

            //룸에 접속한다.
            this.connectionInfoText.text = "룸에 접속....";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            this.connectionInfoText.text = "오프라인 : 마스터 서버와 연결 끊김 \n 다시 접속 시도합니다.";
            //설정한 정보로 마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.connectionInfoText.text = "빈 방 없음, 새로운방 생성...";
        //최대 인원을 4명으로 설정 + 방을 만듦
        //방이름 , 4명 설정
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });

    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        this.connectionInfoText.text = "방 참가 성공!";

        //모든 룸 참가자가 Main 씬을 로드하게 함
        PhotonNetwork.LoadLevel("Main");
    }
}
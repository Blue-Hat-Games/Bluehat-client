using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// 포톤을 이용하여 게임서버까지 들어가게 하는 것이 목적
public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.NickName = "Minjujuu";
        // 이후에 들어온 플레이어에게 현재 씬 상황을 자동으로 적용시켜줌
        PhotonNetwork.AutomaticallySyncScene = true; 
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedMaster");
        // TypedLobby에 따라 어느 룸에 들어갈지 달라짐
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        // 게임서버로 들어가자
        string roomName = "Default Room";
        // 테스트 버전의 MAxPlayers는 20까지만 가능
        RoomOptions ro = new RoomOptions() { MaxPlayers = 4, IsVisible = true, IsOpen = true };

        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // 게임서버로 들어옴. 이곳에서 접속하는 사용자 위치시켜주기
        Debug.Log("OnJoinedRoom");
        // 360도 Sphere 공간안에서 랜덤으로 한 점을 찍은 것
        Vector3 randPos = Random.insideUnitSphere * 10; 
        // 0,0에서 10m 사이 까지의 거리 중 랜덤으로 설정
        randPos.y = 0;
        // 클라이언트가 새로 방에 들어오면 마스터 클라이언트가 자동으로 환경을 맞춰줌 
        PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed");
    }
}

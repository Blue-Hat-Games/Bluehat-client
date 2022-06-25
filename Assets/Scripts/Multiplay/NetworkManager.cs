using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// ������ �̿��Ͽ� ���Ӽ������� ���� �ϴ� ���� ����
public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.NickName = "Minjujuu";
        // ���Ŀ� ���� �÷��̾�� ���� �� ��Ȳ�� �ڵ����� ���������
        PhotonNetwork.AutomaticallySyncScene = true; 
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedMaster");
        // TypedLobby�� ���� ��� �뿡 ���� �޶���
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        // ���Ӽ����� ����
        string roomName = "Default Room";
        // �׽�Ʈ ������ MAxPlayers�� 20������ ����
        RoomOptions ro = new RoomOptions() { MaxPlayers = 4, IsVisible = true, IsOpen = true };

        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // ���Ӽ����� ����. �̰����� �����ϴ� ����� ��ġ�����ֱ�
        Debug.Log("OnJoinedRoom");
        // 360�� Sphere �����ȿ��� �������� �� ���� ���� ��
        Vector3 randPos = Random.insideUnitSphere * 10; 
        // 0,0���� 10m ���� ������ �Ÿ� �� �������� ����
        randPos.y = 0;
        // Ŭ���̾�Ʈ�� ���� �濡 ������ ������ Ŭ���̾�Ʈ�� �ڵ����� ȯ���� ������ 
        // - ù��° �Ű������� Resources���� � �������� �ҷ��� �������� ��Ÿ�� (����,����Ʈ �Ѵ�)
        // PhotonNetwork.Instantiate("Player", randPos, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed");
    }
}

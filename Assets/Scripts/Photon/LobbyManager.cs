using Photon.Pun; // ����Ƽ�� ���� ������Ʈ��
using Photon.Realtime; // ���� ���� ���� ���̺귯��
using UnityEngine;
using UnityEngine.UI;

// ������(��ġ ����ŷ) ������ �� ������ ���
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // ���� ����

    public Text connectionInfoText; // ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Button joinButton; // �� ���� ��ư

    // ���� ����� ���ÿ� ������ ���� ���� �õ�
    private void Start()
    {

        //���ӿ� �ʿ��� ���� (���� ����) ����
        PhotonNetwork.GameVersion = this.gameVersion;
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();


        this.joinButton.interactable = false;
        this.connectionInfoText.text = "������ ������ ������...";
    }

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        this.joinButton.interactable = true;
        this.connectionInfoText.text = "�¶��� : ������ ������ ���� ��";
    }

    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        this.joinButton.interactable = false;
        this.connectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ���... ";
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // �� ���� �õ�
    public void Connect()
    {
        // �ߺ� ���� ����
        this.joinButton.interactable = false;

        // ������ ������ ���� ���̶��
        if (PhotonNetwork.IsConnected)
        {

            //�뿡 �����Ѵ�.
            this.connectionInfoText.text = "�뿡 ����....";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            this.connectionInfoText.text = "�������� : ������ ������ ���� ���� \n �ٽ� ���� �õ��մϴ�.";
            //������ ������ ������ ���� ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (�� ���� ����)���� �� ������ ������ ��� �ڵ� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.connectionInfoText.text = "�� �� ����, ���ο�� ����...";
        //�ִ� �ο��� 4������ ���� + ���� ����
        //���̸� , 4�� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });

    }

    // �뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        this.connectionInfoText.text = "�� ���� ����!";

        //��� �� �����ڰ� Main ���� �ε��ϰ� ��
        PhotonNetwork.LoadLevel("Main");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace BluehatGames {
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private byte maxPlayersPerRoom = 8;
    private bool isConnecting = false;

    [Header("UI")]
    public InputField nickNameInputField;
    public Button connectButton;
    public GameObject controlPanel; // inputField, PlayButton
    public GameObject progressPanel; // connecting ...
    void Awake()
    {
        // ���Ŀ� ���� �÷��̾�� ���� �� ��Ȳ�� �ڵ����� ���������
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    void Start()
    {
        controlPanel.SetActive(true);
        progressPanel.SetActive(false);
        Debug.Log("00. ���� �Ŵ��� ����");

        // Connect ��ư�� ���� ��� ���� �õ� 
        // - �г��� ������ PlayerNameInputField���� ����
        connectButton.onClick.AddListener(() =>
        {
            Connect();
        });
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("01. ���� ������ ����");
        if (isConnecting)
        {
            // �����ϴ� �뿡 �켱 ������ �õ��ϰ�, ���ٸ� OnJoinRandomFailed() �� ȣ���
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    // JoinRandomRoom()�� �����ϸ� ȣ���
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("02. ���� �� ���� ����");

        // �� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = maxPlayersPerRoom; // 8������� ���尡���ϰ� ���� 

        // ���� ���� �� ���忡 �����ϸ�, ���� ���ų� ���� ������ ���� �̷� �� ���ο� ���� ������
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. �� ���� �Ϸ�");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04. �� ���� �Ϸ�");        
        if(PhotonNetwork.IsMasterClient)
        {
            // �� ������ ���� �� �ε�
            PhotonNetwork.LoadLevel(SceneName._PhotonNetworkScene);
            StartCoroutine(RepeatIsConnect());
        }
    }

    IEnumerator RepeatIsConnect() {
        while (PhotonNetwork.LevelLoadingProgress < 1 ){ 
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
    
    // 1. connection ���� ����
    // - �̹� ����Ǿ��ٸ�, ������ ���� �õ�
    // - ������� �ʾҴٸ�, Photon cloud network�� ������ �õ�
    public void Connect()
    {
        Debug.Log("------------------------ Connect ------------------");
        // �����Ϸ��� �ǻ縦 ��� ����. ���ӿ��� ���ƿ� �� ����Ǿ��ٴ� �ݹ��� ���� ���̱� ������ ��� �ؾ� �ϴ��� �˾ƾ� �ϱ� ����
        isConnecting = true;

        controlPanel.SetActive(false);
        progressPanel.SetActive(true);

        // ����Ǿ����� �ƴ��� Ȯ���ؼ� �����ϰų� ������ ������ �õ�
        if (PhotonNetwork.IsConnected)
        {
            // ������ ���� �õ�
            // - �����ϸ� OnJoinRandomFailed()�� ȣ��ǰ� �ϳ��� ���� �� ����        
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // ���� ���� Photon Online Server�� �����ؾ� ��
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

}
}
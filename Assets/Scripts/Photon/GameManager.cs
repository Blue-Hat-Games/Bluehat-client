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

        // ���� �÷��̾ ���� ���� �� ȣ��ǰ�, 'Launcher' ���� �ε��� 
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        #endregion


        #region Public Methods
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public static GameManager Instance;

        /* �� �޼ҵ�� ��������� �÷��̾ Photon Network �뿡�� �������� �ϸ� 
         * �߻�ȭ�� ���ؼ� public �޼ҵ�� wrap �Ͽ����ϴ�. 
         * ���� �ܰ迡���� ������ ���� �Ǵ� Ȯ�� �ܰ� �߰��� �� ���� ��� �߰��� ���� �� �Դϴ�.
         * */
        
        // 'Leave Button' ��Ŭ�� �޼���
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
                // ���� �÷��̾� �ν��Ͻ��� ������,
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
            /* PhotonNetwork.LoadLevel() �� ������ Ŭ���̾�Ʈ�� ��쿡�� ȣ�� �Ǿ�� �մϴ�. 
             * ���� PhotonNetwork.isMasterClient �� �̿��Ͽ� ������ Ŭ���̾���Ʈ������ üũ �մϴ�. 
             * �̰��� üũ�ϴ� ���� ȣ������ å���� �� �� �̸� �� ������ ���� ��Ʈ���� �ٷ� �� �Դϴ�.
             * PhotonNetwork.LoadLevel() �� �̿��Ͽ� ���ϴ� ������ �ε� �մϴ�. 
             * �� ���ӿ����� PhotonNetwork.automaticallySyncScene �� ����ϵ��� �س��ұ� ������ 
             * �� ���� ��� ������ Ŭ���̾�Ʈ�� ���� �� ���� �ε带 ����Ƽ�� ���� �ϴ� ���� �ƴ� Photon �� �ϵ��� �Ͽ����ϴ�.
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

            // PhotonNetwork.isMasterClient �� �̿��Ͽ� �������� ��쿡�� LoadArena() �� ȣ�� �� �� �Դϴ�
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion
    }
}
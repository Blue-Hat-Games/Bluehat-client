using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace BluehatGames
{
    public class MultiplayGameManager : MonoBehaviour
    {
        public static MultiplayGameManager instance = null;
        private bool isConnect = false;
        public Transform spawnPoint;

        public string playerPrefabPath;
        public GameObject cameraPrefab;
        private string selectedAnimal;
        public void SetPlayerPrefabPath(string animalName)
        {
            Debug.Log($"setPlayerPrefabPath -> {animalName}");
            playerPrefabPath = $"Prefab/Animals/{animalName}";
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }

        }

        public void GameOver()
        {

        }

        void Start()
        {
            string animalName = PlayerPrefs.GetString(PlayerPrefsKey.key_multiplayAnimal);
            SetPlayerPrefabPath(animalName);

            StartCoroutine(CreatePlayer());
        }

        public bool IsConnectTrue()
        {
            Debug.Log("IsConnectTrue");
            return isConnect;
        }

        public void SetIsConnectTrue()
        {
            Debug.Log("SetIsConnectTrue");
            this.isConnect = true;
        }

        IEnumerator CreatePlayer()
        {
            Debug.Log("MultiplayGameManager => CreatePlayer()");
            // isConnect = true
            // yield return new WaitUntil(() => isConnect);
            // TODO: 로딩이 다 되고나서 되어야 하는데, 관련 포톤 함수를 못찾아서 일단 3초 정도 지연 후 Create 되도록 함
            yield return new WaitForSeconds(3); // TEST 
            PlayerStatusController.instance.SetStartTimeAttack();
            // 360도 Sphere 공간안에서 랜덤으로 한 점을 찍은 것
            Vector3 adjustedPos = spawnPoint.position;
            Vector3 randPos = Random.insideUnitSphere * 5;
            // 0,0에서 10m 사이 까지의 거리 중 랜덤으로 설정
            adjustedPos = new Vector3(adjustedPos.x + randPos.x, adjustedPos.y, adjustedPos.z + randPos.z);
            
            // 클라이언트가 새로 방에 들어오면 마스터 클라이언트가 자동으로 환경을 맞춰줌 
            GameObject playerTemp = PhotonNetwork.Instantiate(playerPrefabPath, adjustedPos, Quaternion.identity);
            SetMultiplayAnimalObject(playerTemp);
            GameObject camera = GameObject.Instantiate(cameraPrefab);
            camera.GetComponent<PlayerCam>().SetCameraTarget(playerTemp);
        }

        private void SetMultiplayAnimalObject(GameObject animalPlayer)
        {

            animalPlayer.AddComponent<MultiplayAnimalController>();
            animalPlayer.AddComponent<PlayerTrigger>();
            animalPlayer.GetComponentInChildren<Animator>().gameObject.AddComponent<PhotonAnimatorView>();
        }

        public void LeaveRoom()
        {
            // PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }


    }
}
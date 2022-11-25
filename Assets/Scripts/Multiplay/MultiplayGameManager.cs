using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace BluehatGames
{
    public class MultiplayGameManager : MonoBehaviour
    {
        public static MultiplayGameManager instance;
        public Transform spawnPoint;

        public string playerPrefabPath;
        public GameObject cameraPrefab;
        public GameObject loadingPanel;

        public GameObject obstacleTriggerParticle;
        public AudioClip eatEffectSound;

        private SelectedAnimalDataCupid cupid;
        private bool isConnect;

        private int myPlayerViewID;
        private string selectedAnimal;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            cupid = FindObjectOfType<SelectedAnimalDataCupid>();
            if (cupid == null)
                Debug.Log("cupid null");
            else
                playerPrefabPath = cupid.GetSelectedAnimalType();

            SetPlayerPrefabPath(playerPrefabPath);

            StartCoroutine(CreatePlayer());
        }

        public void SetPlayerPrefabPath(string animalName)
        {
            Debug.Log($"setPlayerPrefabPath -> {animalName}");
            playerPrefabPath = $"Prefab/MultiplayAnimals/{animalName}";
        }

        public void GameOver()
        {
        }

        public bool IsConnectTrue()
        {
            Debug.Log("IsConnectTrue");
            return isConnect;
        }

        public void SetIsConnectTrue()
        {
            Debug.Log("SetIsConnectTrue");
            isConnect = true;
        }

        public int GetMyPlayerPhotonViewID()
        {
            return myPlayerViewID;
        }

        private IEnumerator CreatePlayer()
        {
            Debug.Log("MultiplayGameManager => CreatePlayer()");
            // isConnect = true
            // yield return new WaitUntil(() => isConnect);
            // TODO: 로딩이 다 되고나서 되어야 하는데, 관련 포톤 함수를 못찾아서 일단 3초 정도 지연 후 Create 되도록 함
            yield return new WaitForSeconds(3); // TEST 

            PlayerStatusController.instance.SetStartTimeAttack();
            // 360도 Sphere 공간안에서 랜덤으로 한 점을 찍은 것
            var adjustedPos = spawnPoint.position;
            var randPos = Random.insideUnitSphere * 5;
            // 0,0에서 10m 사이 까지의 거리 중 랜덤으로 설정
            adjustedPos = new Vector3(adjustedPos.x + randPos.x, adjustedPos.y, adjustedPos.z + randPos.z);

            // 클라이언트가 새로 방에 들어오면 마스터 클라이언트가 자동으로 환경을 맞춰줌 
            var playerTemp = PhotonNetwork.Instantiate(playerPrefabPath, adjustedPos, spawnPoint.rotation);
            SetMultiplayAnimalObject(playerTemp);
            var camera = Instantiate(cameraPrefab);
            camera.GetComponent<MultiplayCameraController>().SetCameraTarget(playerTemp);
            myPlayerViewID = playerTemp.GetComponent<PhotonView>().ViewID;

            loadingPanel.SetActive(false);
        }

        private void SetMultiplayAnimalObject(GameObject animalPlayer)
        {
            Debug.Log($"Create animalPlayer | name = {animalPlayer.name}");
            if (cupid != null)
            {
                cupid.SetAnimalTexture(animalPlayer);
                cupid.SetHatObject(animalPlayer);
            }

            var playerTrigger = animalPlayer.AddComponent<PlayerTrigger>();
            playerTrigger.SetEatEffectAudioClip(eatEffectSound);
            // animalPlayer.GetComponentInChildren<Animator>().gameObject.AddComponent<PhotonAnimatorView>();
        }

        public void LeaveRoom()
        {
            // PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        public GameObject GetObstacleTriggerParticle()
        {
            return Instantiate(obstacleTriggerParticle, Vector3.zero, Quaternion.identity);
        }
    }
}
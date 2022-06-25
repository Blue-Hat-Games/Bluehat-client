using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace BluehatGames{
public class MultiplayGameManager : MonoBehaviour
{
    public static MultiplayGameManager instance = null;
    private bool isConnect = false;
    
    public string playerPrefabPath;
    public GameObject cameraPrefab;
    private string selectedAnimal;
    public void SetPlayerPrefabPath(string animalName) {
        Debug.Log($"setPlayerPrefabPath -> {animalName}");
        playerPrefabPath = $"Prefab/MultiplayAnimal/{animalName}";
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }

    public void GameOver() {

    }

    void Start()
    {
        string animalName = PlayerPrefs.GetString(PlayerPrefsKey.key_multiplayAnimal);
        SetPlayerPrefabPath(animalName);

        StartCoroutine(CreatePlayer());
    }

    public bool IsConnectTrue() {
        Debug.Log("IsConnectTrue");
        return isConnect;
    }

    public void SetIsConnectTrue() {
        Debug.Log("SetIsConnectTrue");
        this.isConnect = true;
    }
    
    IEnumerator CreatePlayer()
    {
        Debug.Log("MultiplayGameManager => CreatePlayer()");
        // isConnect = true
        // yield return new WaitUntil(() => isConnect);
        // TODO: �ε��� �� �ǰ��� �Ǿ�� �ϴµ�, ���� ���� �Լ��� ��ã�Ƽ� �ϴ� 3�� ���� ���� �� Create �ǵ��� ��
        yield return new WaitForSeconds(3); // TEST 
        PlayerStatusController.instance.SetStartTimeAttack();
        // 360�� Sphere �����ȿ��� �������� �� ���� ���� ��
        Vector3 randPos = Random.insideUnitSphere * 10;
        // 0,0���� 10m ���� ������ �Ÿ� �� �������� ����
        randPos.y = 0;
        // Ŭ���̾�Ʈ�� ���� �濡 ������ ������ Ŭ���̾�Ʈ�� �ڵ����� ȯ���� ������ 
        GameObject playerTemp = PhotonNetwork.Instantiate(playerPrefabPath, randPos, Quaternion.identity);
        GameObject camera = GameObject.Instantiate(cameraPrefab);
        camera.GetComponent<PlayerCam>().SetCameraTarget(playerTemp);
    }
}
}
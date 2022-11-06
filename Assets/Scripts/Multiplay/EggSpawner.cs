using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    // 랜덤의 알을 몇몇 랜덤의 위치에 생성한다.
    // 그 알을 먹으면 메인에서 깔 수 있게 해줌

    // 알 저장은 뭘로 하지? 그냥 playerpref로 개수 저장해주자

    public Transform[] spawnPoints;
    public GameObject eggPrefab;

    void Start()
    {   
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject egg = Instantiate(eggPrefab, spawnPoints[i].position, Quaternion.identity);
            egg.transform.eulerAngles = new Vector3(-90, 0, 0);
        }
    }

}

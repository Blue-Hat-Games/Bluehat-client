using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneAnimal : MonoBehaviour
{
    // 이 스크립트는 메인씬에 배치될 동물에게 붙을 것

    // 메인 씬의 동물들은 적당히 전시용으로 플레이어에게 보여지면 되는데
    // 너무 가만히 있어서도 안되고 좀 자연스럽게 필드를 돌아다녀야 함
    // 나중에는 플레이어가 동물을 터치하면 뭔가 좀 다른 움직임을 하게 하고 싶긴 함 

    // 제어해야 하는 것
    // 1. 애니메이터 컨트롤러
    // 2. 이동 (속도, 방향)
    private Animator animalAnim;
    public enum AnimalState
    {
        Idle,
        Move
    }

    public AnimalState animalState;
    private IEnumerator idleCoroutine;
    private IEnumerator moveCoroutine;

    public float animalMoveSpeed = 10;
    void Start()
    {
        animalAnim = this.gameObject.GetComponentInChildren<Animator>();
        animalState = AnimalState.Idle;

        idleCoroutine = null;
        moveCoroutine = null;
    }

    void Update()
    {
        switch(animalState)
        {
            case AnimalState.Idle:
                moveCoroutine = null;
                if(idleCoroutine == null)
                {
                    // 가만히 서있다가 랜덤 시간 이후에 Move로 변경
                    idleCoroutine = SetIdleStateTimer();
                    StartCoroutine(idleCoroutine);
                }
            break;
            case AnimalState.Move:
                idleCoroutine = null;
                if(moveCoroutine == null)
                {
                    moveCoroutine = SetMoveStateTimer();
                    StartCoroutine(moveCoroutine);
                }
            break;
        }
    }


    IEnumerator SetIdleStateTimer()
    {
        float randomTimer = Random.RandomRange(5, 15);
        yield return new WaitForSeconds(randomTimer);
        animalState = AnimalState.Move;
    }

    IEnumerator SetMoveStateTimer()
    {
        // 랜덤의 방향으로 랜덤한 만큼 이동
        // Quaternion randRotate = Random.rotationUniform;
        // this.gameObject.transform.rotation = randRotate;

        float randomTimer = Random.RandomRange(5, 15);
        float timer = 0;

        float randomX = Random.Range(0.0f, 1.0f);
        float randomZ = Random.Range(0.0f, 1.0f);

        Debug.Log($"Random Rot = ${randomX}, ${randomZ}");

        Vector3 randomRot = new Vector3(randomX, 0, randomZ);

        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            if(timer > randomTimer)
            {
                animalState = AnimalState.Idle;
            }

            
            this.gameObject.transform.Translate(randomRot * animalMoveSpeed * Time.deltaTime);
        }
    } 
}

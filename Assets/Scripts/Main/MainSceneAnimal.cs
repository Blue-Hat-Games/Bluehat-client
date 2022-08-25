using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneAnimal : MonoBehaviour
{
    // �� ��ũ��Ʈ�� ���ξ��� ��ġ�� �������� ���� ��

    // ���� ���� �������� ������ ���ÿ����� �÷��̾�� �������� �Ǵµ�
    // �ʹ� ������ �־�� �ȵǰ� �� �ڿ������� �ʵ带 ���ƴٳ�� ��
    // ���߿��� �÷��̾ ������ ��ġ�ϸ� ���� �� �ٸ� �������� �ϰ� �ϰ� �ͱ� �� 

    // �����ؾ� �ϴ� ��
    // 1. �ִϸ����� ��Ʈ�ѷ�
    // 2. �̵� (�ӵ�, ����)
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
                    // ������ ���ִٰ� ���� �ð� ���Ŀ� Move�� ����
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
        // ������ �������� ������ ��ŭ �̵�
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

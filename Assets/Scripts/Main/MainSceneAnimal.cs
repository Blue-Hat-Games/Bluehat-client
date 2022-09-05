using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneAnimal : MonoBehaviour
{
    // �� ��ũ��Ʈ�� ���ξ��� ��ġ�� �������� ���� ��

    // ���� ���� �������� ������ ���ÿ����� �÷��̾�� �������� �Ǵµ�
    // �ʹ� ������ �־�� �ȵǰ� �� �ڿ������� �ʵ带 ���ƴٳ�� ��
    // ���߿��� �÷��̾ ������ ��ġ�ϸ� ���� �� �ٸ� �������� �ϰ� �ϰ� �ͱ� �� 

    private const string ANIM_PARAMETER_JUMP = "Jump";
    private const string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";

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

    public float animalMoveSpeed = 0.5f;


    private float idleToWalkTransitionValue = 0.2f;
    private float walkToIdleTransitionValue = 0.08f;

    private Vector2 moveLimit = new Vector2(5, 0);
    
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
        float randomTimer = Random.RandomRange(3, 10);
        animalAnim.SetFloat(ANIM_PARAMETER_MOTIONSPEED, walkToIdleTransitionValue);
        yield return new WaitForSeconds(randomTimer);
        animalState = AnimalState.Move;
        yield break;
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

        Vector3 randomRot = new Vector3(randomX, 0, randomZ);

        Vector3 curPos = this.transform.position;
        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            if(timer > randomTimer)
            {
                animalState = AnimalState.Idle;
                yield break;
            }

            // �ִϸ����� �Ķ���� ���� 
            animalAnim.SetFloat(ANIM_PARAMETER_MOTIONSPEED, idleToWalkTransitionValue);

            // �̵� ���� 
            this.gameObject.transform.localPosition = ClampPosition(this.transform.localPosition);

            this.gameObject.transform.rotation = Quaternion.LookRotation(randomRot - curPos);
            this.gameObject.transform.Translate(randomRot * animalMoveSpeed * Time.deltaTime);

        }
    } 

    public Vector3 ClampPosition(Vector3 pos)
    {
        return new Vector3 
        (
            Mathf.Clamp(pos.x, -moveLimit.x, moveLimit.x),
            this.transform.position.y,
            Mathf.Clamp(pos.z, -moveLimit.x, moveLimit.x)
        );
    }
}

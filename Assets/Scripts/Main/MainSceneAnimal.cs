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

    private Rigidbody rigidl;
    public AnimalState animalState;
    private IEnumerator idleCoroutine;
    private IEnumerator moveCoroutine;

    [SerializeField] private Vector3 direction;
    [SerializeField] private float animalMoveSpeed = 10;

    private float idleToWalkTransitionValue = 0.2f;
    private float walkToIdleTransitionValue = 0.08f;
    

    void Start()
    {
        rigidl = this.gameObject.GetComponent<Rigidbody>();
        animalAnim = this.gameObject.GetComponentInChildren<Animator>();
        animalState = AnimalState.Idle;

        idleCoroutine = null;
        moveCoroutine = null;

        // ȸ�� �� ���� ���� 
        this.transform.eulerAngles = new Vector3(0, Random.Range(0, 360f), 0);
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
        float randomTimer = Random.Range(3, 10);
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

        float randomTimer = Random.Range(5, 15);
        float timer = 0;

        float randomX = Random.Range(0.0f, 1.0f);
        float randomZ = Random.Range(0.0f, 1.0f);

        Vector3 randomDir = new Vector3(randomX, 0, randomZ);

        Vector3 curPos = this.transform.position;
        // ���� ����
        direction.Set(0f, Random.Range(0f, 360f), 0f);

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

            rigidl.MovePosition(transform.position + transform.forward * animalMoveSpeed * Time.deltaTime);

            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, direction, 0.01f);
            rigidl.MoveRotation(Quaternion.Euler(_rotation));
            // this.gameObject.transform.LookAt(randomDir - curPos);
            // // this.gameObject.transform.rotation = Quaternion.LookRotation(randomDir - curPos);
            // this.gameObject.transform.Translate(randomDir * animalMoveSpeed * Time.deltaTime);

        }
    } 

    // public Vector3 ClampPosition(Vector3 pos)
    // {
    //     return new Vector3 
    //     (
    //         Mathf.Clamp(pos.x, -moveLimit.x, moveLimit.x),
    //         this.transform.position.y,
    //         Mathf.Clamp(pos.z, -moveLimit.x, moveLimit.x)
    //     );
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneAnimal : MonoBehaviour
{
    // 이 스크립트는 메인씬에 배치될 동물에게 붙을 것

    // 메인 씬의 동물들은 적당히 전시용으로 플레이어에게 보여지면 되는데
    // 너무 가만히 있어서도 안되고 좀 자연스럽게 필드를 돌아다녀야 함
    // 나중에는 플레이어가 동물을 터치하면 뭔가 좀 다른 움직임을 하게 하고 싶긴 함 

    private const string ANIM_PARAMETER_JUMP = "Jump";
    private const string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";

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
        float randomTimer = Random.RandomRange(3, 10);
        animalAnim.SetFloat(ANIM_PARAMETER_MOTIONSPEED, walkToIdleTransitionValue);
        yield return new WaitForSeconds(randomTimer);
        animalState = AnimalState.Move;
        yield break;
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

            // 애니메이터 파라미터 설정 
            animalAnim.SetFloat(ANIM_PARAMETER_MOTIONSPEED, idleToWalkTransitionValue);

            // 이동 제한 
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

using Photon.Pun;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField] private float directionDampTime = 0.25f;

        #endregion


        #region MonoBehaviour Callbacks

        private Animator animator;

        // Use this for initialization
        private void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator) Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }


        // Update is called once per frame
        private void Update()
        {
            /* PhotonNetwork.IsConnected == true 가 있어야 할까요? 예. :) 
             * 개발 단계에서 이 프리팹을 연결 없이 테스트 하고 싶을 수도 있기 때문입니다. 
             * 예를 들어 더미 신에서 네트워킹 자체 기능에 관련되지 않은 것을 생성하고 코드를 검증 하기 위해서 입니다. 
             * 이 추가적인 표현식을 통해서 연결되지 않았어도 입력을 허용하도록 할 것 입니다. 매우 간단한 트릭이고 개발 기간동안에 작업흐름을 좋게 할 수 있습니다.
             * */
            if (photonView.IsMine == false && PhotonNetwork.IsConnected) return;
            if (!animator) return;

            // deal with Jumping
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
                // When using trigger parameter
                if (Input.GetMouseButtonDown(1))
                    animator.SetTrigger("Jump");

            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            if (v < 0) v = 0;
            /*두 입력값을 제곱하고 있다는 것을 알아 챘을 것 입니다.
             * 왜 그럴까요? 항상 양의 절대 값이고 easing을 추가하기 때문 입니다. 
             * 절묘한 트릭입니다. Mathf.Abs() 도 역시 사용할 수 있으며 잘 동작 합니다.
             * */
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }
}
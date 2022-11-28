using Photon.Pun;
using UnityEngine;

namespace BluehatGames
{
    public class MultiplayAnimalController : MonoBehaviourPun, IPunObservable
    {
        public float rotSpeed = 10;

        private readonly string ANIM_PARAMETER_JUMP = "Jump";
        private readonly string ANIM_PARAMETER_MOTIONSPEED = "MotionSpeed";
        private Animator animator;
        private float camAngle;
        private SelectedAnimalDataCupid cupid;
        private bool isCompleted = false;

        private bool isFirst = true;

        private bool isGround = true;
        protected Joybutton joybutton;

        protected Joystick joystick;
        private readonly float jumpPower = 6;

        private float moveSpeed = 15;

        private MultiplayCameraController multiplayCameraController;
        private Quaternion remoteCamRot = Quaternion.identity;

        // 받은 데이터 기억 변수 (보간처리하기 위해서)
        private Vector3 remotePos = Vector3.zero;
        private Quaternion remoteRot = Quaternion.identity;
        private Vector3 remoteScale = Vector3.zero;

        private Rigidbody rigid;

        private void Start()
        {
            joystick = FindObjectOfType<Joystick>();
            joybutton = FindObjectOfType<Joybutton>();

            rigid = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();

            multiplayCameraController = FindObjectOfType<MultiplayCameraController>();
            if (multiplayCameraController)
            {
                var CameraTr = multiplayCameraController.GetCameraTransform();
                var lookForward = new Vector3(CameraTr.forward.x, 0f, CameraTr.forward.z).normalized;
                var lookRight = new Vector3(CameraTr.right.x, 0f, CameraTr.right.z).normalized;
                var moveDir = lookForward * 0 + lookRight * 0;
            }

            var pv = gameObject.GetComponent<PhotonView>();
            Debug.Log($"pv object = {gameObject.name}, pv.IsMine = {pv.IsMine}");
            cupid = FindObjectOfType<SelectedAnimalDataCupid>();
            if (pv.IsMine)
            {
                var data = cupid.GetSelectedAnimalData();
                var json = JsonUtility.ToJson(data);
                pv.RPC("SetAnimalCostume", RpcTarget.AllBuffered, json);
            }
        }

        private void Update()
        {
            if ((transform.localScale.x == 0) | (transform.localScale.y == 0) || transform.localScale.z == 0)
                transform.localScale = new Vector3(1, 1, 1);
            // 리모트 캐릭터 처리
            if (photonView.IsMine == false)
            {
                ControlRemotePlayer();
                return;
            }

            if (PlayerStatusController.instance.IsGameOver()) return;

            // 애니메이터 파라미터 설정 
            animator.SetFloat(ANIM_PARAMETER_MOTIONSPEED, joystick.InputScale);

            var h = joystick.Horizontal;
            var v = joystick.Vertical;

            var dir = new Vector3(h, 0, v);
            // Debug.Log($"joystick h = {h}, v = {v}");
            // Debug.Log($"joystick dir = {dir}");
            // rigid.velocity = new Vector3(h * moveSpeed, rigid.velocity.y, v * moveSpeed);

            if (!(h == 0 && v == 0)) // 조이스틱에 값이 들어오고 있을 때 
            {
                var CameraTr = multiplayCameraController.GetCameraTransform();

                var lookForward = new Vector3(CameraTr.forward.x, 0f, CameraTr.forward.z).normalized;
                var lookRight = new Vector3(CameraTr.right.x, 0f, CameraTr.right.z).normalized;
                var moveDir = lookForward * v + lookRight * h;
                // Debug.Log($"moveDir => {moveDir}");
                transform.forward = moveDir;
                transform.position += moveDir * Time.deltaTime * 5f;

                return;
            }

            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);

            // Jump에 대한 처리
            if (isGround && joybutton.Pressed)
            {
                rigid.velocity = Vector3.up * jumpPower;
                animator.SetTrigger(ANIM_PARAMETER_JUMP);
                isGround = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 부딪힌 물체의 태그가 "Ground"라면
            if (collision.gameObject.CompareTag("Ground"))
                // isGround를 true로 변경
                isGround = true;
        }

        // IPunObservable 상속 시 꼭 구현해야 하는 것
        // - 데이터를 네트워크 사용자 간에 보내고 받고 하게 하는 콜백 함수
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 내가 데이터를 보내는 중이라면
            if (stream.IsWriting) // 내꺼보내는 거
            {
                // 이 방안에 있는 모든 사용자에게 브로드캐스트 
                // - 내 포지션 값을 보내보자
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(transform.localScale);
                Debug.Log($"stream.SendNext | scale = {transform.localScale}");
            }
            // 내가 데이터를 받는 중이라면 
            else // 원격에 있는 나 
            {
                // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
                remotePos = (Vector3)stream.ReceiveNext();
                remoteRot = (Quaternion)stream.ReceiveNext();
                remoteScale = (Vector3)stream.ReceiveNext();

                Debug.Log($"stream.ReceiveNext | remoteScale => {remoteScale}");
            }
        }

        private void ControlRemotePlayer()
        {
            transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, remoteScale, 10 * Time.deltaTime);
            if ((transform.localScale.x == 0) | (transform.localScale.y == 0) || transform.localScale.z == 0)
                transform.localScale = new Vector3(1, 1, 1);
        }

        [PunRPC]
        public void SetAnimalCostume(string jsonData)
        {
            var dataForm = JsonUtility.FromJson<AnimalDataFormat>(jsonData);
            Debug.Log($"ChatMessage {dataForm.id}, {dataForm.animalType}, thisGameObject = {gameObject.name}");
            if (cupid == null) cupid = FindObjectOfType<SelectedAnimalDataCupid>();
            cupid.LoadHatItemPrefab(dataForm.headItem, gameObject);
            cupid.SetRemoteAnimalTexture(dataForm, gameObject);
        }
    }
}
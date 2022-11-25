using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    /// <summary>
    ///     Player manager.
    ///     Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Private Fields

        [Tooltip("The Beams GameObject to control")] [SerializeField]
        private GameObject beams;

        //True, when the user is firing
        private bool IsFiring;

        #endregion

        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")] [SerializeField]
        private GameObject playerUiPrefab;

        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                IsFiring = (bool)stream.ReceiveNext();
                Health = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (beams == null)
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            else
                beams.SetActive(false);

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine) LocalPlayerInstance = gameObject;
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            // 레벨 씬이 로드될 때 인스턴스가 살아남도록 함 
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // CameraWork 를 불러옴
            var _cameraWork = gameObject.GetComponent<CameraWork>();
            if (_cameraWork != null)
            {
                // 해당 뷰가 로컬 클라이언트이면 카메라가 따라다니도록 함 
                if (photonView.IsMine) _cameraWork.OnStartFollowing();
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            if (playerUiPrefab != null)
            {
                var _uiGo = Instantiate(playerUiPrefab);
                // 대상의 SetTarget 메서드를 호출함
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            SceneManager.sceneLoaded += (scene, loadingMode) => { CalledOnLevelWasLoaded(scene.buildIndex); };
#endif
        }

#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        private void CalledOnLevelWasLoaded(int level)
        {
            var _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)) transform.position = new Vector3(0f, 5f, 0f);
        }

        /// <summary>
        ///     MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        private void Update()
        {
            //ProcessInputs();
            if (photonView.IsMine)
            {
                ProcessInputs();
                if (Health <= 0f) GameManager.Instance.LeaveRoom();
            }

            // trigger Beams active state
            if (beams != null && IsFiring != beams.activeInHierarchy) beams.SetActive(IsFiring);
        }

        /// <summary>
        ///     MonoBehaviour method called when the Collider 'other' enters the trigger.
        ///     Affect Health of the Player if the collider is a beam
        ///     Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        ///     One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return;
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam")) return;
            Health -= 0.1f;
        }

        /// <summary>
        ///     MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        ///     We're going to affect health while the beams are touching the player
        /// </summary>
        /// <param name="other">Other.</param>
        private void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine) return;
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam")) return;
            // we slowly affect health when beam is co
            // nstantly hitting us, so player has to move to prevent death.
            Health -= 0.1f * Time.deltaTime;
        }

        #endregion

        #region Custom

        /// <summary>
        ///     Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        private void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
                if (!IsFiring)
                    IsFiring = true;
            if (Input.GetButtonUp("Fire1"))
                if (IsFiring)
                    IsFiring = false;
        }

        #endregion
    }
}
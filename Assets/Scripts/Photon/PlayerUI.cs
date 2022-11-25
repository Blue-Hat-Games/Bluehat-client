using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Public Fields

        [Tooltip("Pixel offset from the player target")] [SerializeField]
        private Vector3 screenOffset = new(0f, 30f, 0f);

        #endregion


        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.",
                    this);
                return;
            }

            // Cache references for efficiency
            target = _target;

            var _characterController = _target.GetComponent<CharacterController>();
            // Get data from the Player that won't change during the lifetime of this Component
            if (_characterController != null) characterControllerHeight = _characterController.height;

            if (playerNameText != null) playerNameText.text = target.photonView.Owner.NickName;
        }

        #endregion

        #region Private Fields

        private PlayerManager target;

        [Tooltip("UI Text to display Player's Name")] [SerializeField]
        private Text playerNameText;


        [Tooltip("UI Slider to display Player's Health")] [SerializeField]
        private Slider playerHealthSlider;

        private float characterControllerHeight;
        private Transform targetTransform;
        private Vector3 targetPosition;

        #endregion


        #region MonoBehaviour CallBacks

        private void Awake()
        {
            transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        private void Update()
        {
            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Reflect the Player Health
            if (playerHealthSlider != null) playerHealthSlider.value = target.Health;
        }

        private void LateUpdate()
        {
            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }

        #endregion
    }
}
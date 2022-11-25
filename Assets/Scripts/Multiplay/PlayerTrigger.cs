using System.Collections;
using Photon.Pun;
using UnityEngine;

// 사용자가 Obstacle이랑 부딪히면 해당 Obstacle이 사라지도록 하고 싶다.
// Obstacle은 PhotonView가 있는 객체여야 함
namespace BluehatGames
{
    public class PlayerTrigger : MonoBehaviourPun
    {
        public float scaleChangeOnce = 0.2f;
        public float scaleAdjustValue = 0.01f;
        public float scaleChangeSeconds = 0.1f;

        private MultiplayCameraController camController;

        private AudioClip eatEffectSound;

        private bool isFirst;

        private GameObject obstacleParticle1;
        private GameObject obstacleParticle2;
        private ParticleSystem obstacleParticleSystem1;
        private ParticleSystem obstacleParticleSystem2;
        private Coroutine scaleCoroutine;
        private PlayerStatusController statusController;

        private void Start()
        {
            statusController = FindObjectOfType<PlayerStatusController>();

            obstacleParticle1 = MultiplayGameManager.instance.GetObstacleTriggerParticle();
            obstacleParticleSystem1 = obstacleParticle1.GetComponent<ParticleSystem>();

            obstacleParticle2 = MultiplayGameManager.instance.GetObstacleTriggerParticle();
            obstacleParticleSystem2 = obstacleParticle1.GetComponent<ParticleSystem>();

            obstacleParticle1.transform.SetParent(transform, false);
            obstacleParticle1.transform.localPosition = new Vector3(0, 1, 0);
            obstacleParticle2.transform.SetParent(transform, false);
            obstacleParticle1.transform.localPosition = new Vector3(0, 1, 0);
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (statusController.IsGameOver()) return;

            if (coll.tag == "Obstacle")
            {
                // 에테르 게이지를 더해준다.
                PlayerStatusController.instance.AddAetherEnergy();

                var pv = coll.gameObject.GetComponent<PhotonView>();
                pv.RPC("ShowOffObstacle", RpcTarget.All);

                scaleCoroutine = StartCoroutine(UpdatePlayerScale());

                if (isFirst)
                    PlayParticle(1, transform.position);
                else
                    PlayParticle(2, transform.position);
                SoundManager.instance.PlayEffectSound(eatEffectSound);
                isFirst = !isFirst;
            }

            // 동물의 알은 네트워크 객체는 아님
            if (coll.tag == "AnimalEgg")
            {
                PlayerStatusController.instance.AddMultiplayEggCount();
                Destroy(coll.gameObject);
            }
        }

        public void SetEatEffectAudioClip(AudioClip clip)
        {
            eatEffectSound = clip;
        }

        private void PlayParticle(int number, Vector3 playPos)
        {
            if (number == 1)
                // obstacleParticle1.transform.position = playPos;
                obstacleParticleSystem1.Play();
            else
                // obstacleParticle2.transform.position = playPos;
                obstacleParticleSystem2.Play();
        }

        public void SetMultiplayCameraController(MultiplayCameraController instance)
        {
            camController = instance;
        }


        private IEnumerator UpdatePlayerScale()
        {
            var curScale = gameObject.transform.localScale;
            var curScaleValue = curScale.x;
            var goalScaleValue = curScale.x + scaleChangeOnce;
            for (var i = curScaleValue; i < goalScaleValue; i += scaleAdjustValue)
            {
                gameObject.transform.localScale = new Vector3(i, i, i);
                UpdateCameraDistance(i);
                yield return new WaitForSeconds(scaleChangeSeconds);
            }
        }

        private void UpdateCameraDistance(float value)
        {
            if (camController == null) camController = FindObjectOfType<MultiplayCameraController>();
            camController.AdjustCamDistance(scaleAdjustValue);
        }
    }
}
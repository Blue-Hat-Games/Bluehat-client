using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 사용자가 Obstacle이랑 부딪히면 해당 Obstacle이 사라지도록 하고 싶다.
// Obstacle은 PhotonView가 있는 객체여야 함
namespace BluehatGames
{
    public class PlayerTrigger : MonoBehaviourPun
    {
        Coroutine scaleCoroutine;
        public float scaleChangeOnce = 0.2f;
        public float scaleAdjustValue = 0.01f;
        public float scaleChangeSeconds = 0.1f;
        
        private GameObject obstacleParticle1;
        private ParticleSystem obstacleParticleSystem1;
        private GameObject obstacleParticle2;
        private ParticleSystem obstacleParticleSystem2;
        
        void Start()
        {
            obstacleParticle1 = MultiplayGameManager.instance.GetObstacleTriggerParticle();
            obstacleParticleSystem1 = obstacleParticle1.GetComponent<ParticleSystem>();

            obstacleParticle2 = MultiplayGameManager.instance.GetObstacleTriggerParticle();
            obstacleParticleSystem2 = obstacleParticle1.GetComponent<ParticleSystem>();

            obstacleParticle1.transform.SetParent(this.transform, false);
            obstacleParticle2.transform.SetParent(this.transform, false);
        }

        private void PlayParticle(int number, Vector3 playPos)
        {
            if(number == 1)
            {
                // obstacleParticle1.transform.position = playPos;
                obstacleParticleSystem1.Play();
            }
            else
            {
                // obstacleParticle2.transform.position = playPos;
                obstacleParticleSystem2.Play();
            }

        }
        bool isFirst = false;
        void OnTriggerEnter(Collider coll)
        {
            if (coll.tag == "Obstacle")
            {
                // 에테르 게이지를 더해준다.
                PlayerStatusController.instance.AddAetherEnergy();
                PhotonView pv = coll.gameObject.GetComponent<PhotonView>();
                if (pv.IsMine)
                {
                    PhotonNetwork.Destroy(coll.gameObject);
                }
                scaleCoroutine = StartCoroutine(UpdatePlayerScale());

                if(isFirst)
                {
                    PlayParticle(1, this.transform.position);
                }
                else
                {
                    PlayParticle(2, this.transform.position);
                } 
                isFirst = !isFirst;
            }

            // 동물의 알은 네트워크 객체는 아님
            if(coll.tag == "AnimalEgg")
            {
                PlayerStatusController.instance.AddMultiplayEggCount();
                GameObject.Destroy(coll.gameObject);
            }
        }


        IEnumerator UpdatePlayerScale()
        {
            Vector3 curScale = this.gameObject.transform.localScale;
            float curScaleValue = curScale.x;
            float goalScaleValue = curScale.x + scaleChangeOnce;
            for(float i = curScaleValue; i < goalScaleValue; i += scaleAdjustValue)
            {
                this.gameObject.transform.localScale = new Vector3(i, i, i);
                yield return new WaitForSeconds(scaleChangeSeconds);
            }
        }
    }
}
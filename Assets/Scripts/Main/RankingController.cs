using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames
{
    public class RankingController : MonoBehaviour
    {
        public Button btn_ranking;
        public GameObject rankingPanel;

        public Button btn_ranking_pannel_close;
        public AudioClip upperButtonSound;


        private void Start()
        {
            btn_ranking.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(upperButtonSound);
                rankingPanel.SetActive(true);
            });

            btn_ranking_pannel_close.onClick.AddListener(() => { rankingPanel.SetActive(false); });
        }
    }
}
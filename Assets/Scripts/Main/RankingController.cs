using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BluehatGames
{
    public class RankingController : MonoBehaviour
    {
        public Button btn_ranking;
        public GameObject rankingPanel;

        public Button btn_ranking_pannel_close;
        public AudioClip upperButtonSound;


        void Start()
        {

            btn_ranking.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(upperButtonSound);
                rankingPanel.SetActive(true);
            });

            btn_ranking_pannel_close.onClick.AddListener(() =>
            {
                rankingPanel.SetActive(false);
            });
        }


    }
}
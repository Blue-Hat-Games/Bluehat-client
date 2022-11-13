using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BluehatGames
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance = null;
        public AudioSource[] audioSources;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public void PlayEffectSound(AudioClip clip)
        {
            SoundUtil soundUtil = new SoundUtil();
            if (soundUtil.isSoundEffectOn())
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    if (audioSources[i].isPlaying == false)
                    {
                        audioSources[i].clip = clip;
                        audioSources[i].Play();

                        break;
                    }
                }
            }

        }
    }

}
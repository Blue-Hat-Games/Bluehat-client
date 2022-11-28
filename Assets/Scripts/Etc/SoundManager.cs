using UnityEngine;

namespace BluehatGames
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        public AudioSource[] audioSources;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void PlayEffectSound(AudioClip clip)
        {
            var soundUtil = new SoundUtil();
            if (soundUtil.isSoundEffectOn())
                for (var i = 0; i < audioSources.Length; i++)
                    if (audioSources[i].isPlaying == false)
                    {
                        audioSources[i].clip = clip;
                        audioSources[i].Play();

                        break;
                    }
        }
    }
}
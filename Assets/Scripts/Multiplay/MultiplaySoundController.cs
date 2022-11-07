using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace BluehatGames
{
    public class MultiplaySoundController : MonoBehaviour
    {
        [Header("Music")]
        public AudioSource audioSource;
        private SoundUtil soundUtil;

        public void Start()
        {
            soundUtil = new SoundUtil();

            if (soundUtil.isbackgroundMusicOn())
            {
                Debug.Log("Sound Is On");
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

}
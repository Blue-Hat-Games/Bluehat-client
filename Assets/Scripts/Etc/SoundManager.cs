using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource audioSource;

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
        audioSource.clip = clip;
        audioSource.Play();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BluehatGames
{
    public class SoundUtil
    {
        public bool isbackgroundMusicOn()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKey.key_BackgroundMusic, 1) == 1;
        }

        public bool isSoundEffectOn()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKey.key_SoundEffect, 1) == 1;
        }

        public void turnOnSoundEffect()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_SoundEffect, 1);
        }
        public void turnOffSoundEffect()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_SoundEffect, 0);
        }

        public void turnOnBackgroundMusic()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_BackgroundMusic, 1);
        }

        public void turnOffBackgroundMusic()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_BackgroundMusic, 0);
        }
    }
}
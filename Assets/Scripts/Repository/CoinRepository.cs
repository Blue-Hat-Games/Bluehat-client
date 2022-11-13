using UnityEngine;


namespace BluehatGames
{
    public static class CoinRepository
    {
        public static int GetCoin()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKey.key_aetherCoin);
        }

        public static void SetCoin(int value)
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_aetherCoin, value);
        }
    }
}
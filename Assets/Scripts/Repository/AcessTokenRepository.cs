using UnityEngine;

namespace BluehatGames
{
    public static class AccessToken
    {
        private const string DefaultToken = "0000";

        public static string GetAccessToken()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.key_accessToken) ||
                PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken) != null)
            {
                var accessToken = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
                return accessToken ?? DefaultToken;
            }

            return DefaultToken;
        }

        public static void SetAccessToken(string value)
        {
            PlayerPrefs.SetString(PlayerPrefsKey.key_accessToken, value);
        }

        public static bool CheckAcessToken()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.key_accessToken) ||
                PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken) != null ||
                PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken) != "")
                return true;
            return false;
        }
    }
}
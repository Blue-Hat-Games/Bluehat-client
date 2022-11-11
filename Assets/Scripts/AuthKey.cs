using UnityEngine;

namespace BluehatGames
{
    public class AuthKey
    {
        public static string GetAuthKey()
        {
            var accessToken = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            return accessToken;
        }

        // Checking if Auth Key is Available
        public static bool CheckAuthKey()
        {
            var accessToken = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            return !string.IsNullOrEmpty(accessToken);
            // Checking Auth Key to Server
            // If Err in Auth Key, Clear Auth Key
        }

        public static void SetAuthKey(string value)
        {
            PlayerPrefs.SetString(PlayerPrefsKey.key_accessToken, value);
        }

        public static void ClearAuthKey()
        {
            PlayerPrefs.SetString(PlayerPrefsKey.key_accessToken, "");
        }
    }
}
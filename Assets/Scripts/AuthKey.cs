using UnityEngine;

namespace BluehatGames
{

    public class AuthKey
    {
        public static string GetAuthKey()
        {
            string access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            return access_token;
        }

        // Checking if Auth Key is Available
        public static bool CheckAuthKey()
        {
            string access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            if (access_token == null || access_token == "")
            {
                return false;
            }
            else
            {
                // Checking Auth Key to Server
                // If Err in Auth Key, Clear Auth Key
                return true;
            }
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
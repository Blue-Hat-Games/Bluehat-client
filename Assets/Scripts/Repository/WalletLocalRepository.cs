using UnityEngine;

namespace BluehatGames
{
    public class WalletLocalRepositroy
    {

        public static void SetWalletAddress(string walletAddress)
        {
            PlayerPrefs.SetString(PlayerPrefsKey.key_WalletAddress, walletAddress);
        }

        public static void SetWalletPrivateKey(string privateKey)
        {
            privateKey = AESCrypto.AESEncrypt128(privateKey);
            PlayerPrefs.SetString(PlayerPrefsKey.key_WalletPrivateKey, privateKey);
        }

        public static string GetWalletAddress()
        {
            return PlayerPrefs.GetString(PlayerPrefsKey.key_WalletAddress);
        }

        public static string GetWallletPrivateKey()
        {
            string privateKey = PlayerPrefs.GetString(PlayerPrefsKey.key_WalletPrivateKey);
            privateKey = AESCrypto.AESDecrypt128(privateKey);
            return privateKey;
        }

    }
}
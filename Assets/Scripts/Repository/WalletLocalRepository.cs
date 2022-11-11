using UnityEngine;

namespace BluehatGames
{
    public static class WalletLocalRepository
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

        public static void setKlaytnWalletKey(string klaytnWalletKey)
        {
            klaytnWalletKey = AESCrypto.AESEncrypt128(klaytnWalletKey);
            PlayerPrefs.SetString(PlayerPrefsKey.key_KlaytnWalletKey, klaytnWalletKey);
        }

        public static void setWalletInfo(Wallet wallet)
        {
            SetWalletAddress(wallet.address);
            SetWalletPrivateKey(wallet.privateKey);
            setKlaytnWalletKey(wallet.klaytnWalletKey);
        }

        public static string GetWalletAddress()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.key_WalletAddress))
                return PlayerPrefs.GetString(PlayerPrefsKey.key_WalletAddress);
            return null;
        }

        public static string GetWallletPrivateKey()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.key_WalletPrivateKey))
            {
                var privateKey = PlayerPrefs.GetString(PlayerPrefsKey.key_WalletPrivateKey);
                privateKey = AESCrypto.AESDecrypt128(privateKey);
                return privateKey;
            }

            return null;
        }

        public static string GetKlaytnWalletKey()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey.key_KlaytnWalletKey))
            {
                var klaytnWalletKey = PlayerPrefs.GetString(PlayerPrefsKey.key_KlaytnWalletKey);
                klaytnWalletKey = AESCrypto.AESDecrypt128(klaytnWalletKey);
                return klaytnWalletKey;
            }

            return null;
        }

        public static Wallet GetWalletInfo()
        {
            var address = GetWalletAddress();
            var privateKey = GetWallletPrivateKey();
            var klaytnWalletKey = GetKlaytnWalletKey();
            if (address != null && privateKey != null && klaytnWalletKey != null)
                return new Wallet(address, privateKey, klaytnWalletKey);
            return null;
        }
    }
}
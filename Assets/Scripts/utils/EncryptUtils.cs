using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BluehatGames
{
    public class AESCrypto
    {
        private static readonly string PASSWORD = "dhn2llkdc72!dasdf";
        private static readonly string KEY = PASSWORD.Substring(0, 128 / 8);
        public static string AESEncrypt128(string plain)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.KeySize = 128;

            MemoryStream memoryStream = new MemoryStream();

            ICryptoTransform encryptor = myRijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] encryptBytes = memoryStream.ToArray();

            string encryptString = Convert.ToBase64String(encryptBytes);

            cryptoStream.Close();
            memoryStream.Close();

            return encryptString;
        }

        public static string AESDecrypt128(string encrypt)
        {
            byte[] encryptBytes = Convert.FromBase64String(encrypt);

            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.KeySize = 128;

            MemoryStream memoryStream = new MemoryStream(encryptBytes);

            ICryptoTransform decryptor = myRijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainBytes = new byte[encryptBytes.Length];

            int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

            string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

            cryptoStream.Close();
            memoryStream.Close();

            return plainString;
        }





    }

}
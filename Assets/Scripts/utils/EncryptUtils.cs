using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BluehatGames
{
    public class AESCrypto
    {
        private static readonly string PASSWORD = "dhn2llkdc72!dasdf";
        private static readonly string KEY = PASSWORD.Substring(0, 128 / 8);

        public static string AESEncrypt128(string plain)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plain);

            var myRijndael = new RijndaelManaged();
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.KeySize = 128;

            var memoryStream = new MemoryStream();

            var encryptor = myRijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            var encryptBytes = memoryStream.ToArray();

            var encryptString = Convert.ToBase64String(encryptBytes);

            cryptoStream.Close();
            memoryStream.Close();

            return encryptString;
        }

        public static string AESDecrypt128(string encrypt)
        {
            var encryptBytes = Convert.FromBase64String(encrypt);

            var myRijndael = new RijndaelManaged();
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.KeySize = 128;

            var memoryStream = new MemoryStream(encryptBytes);

            var decryptor = myRijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            var plainBytes = new byte[encryptBytes.Length];

            var plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

            var plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

            cryptoStream.Close();
            memoryStream.Close();

            return plainString;
        }
    }
}
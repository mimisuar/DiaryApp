using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;

namespace Diary.Server.Utils
{
    public static class Crypto
    {
        private static readonly byte[] salt = [0, 100, 50, 32, 100, 20, 40, 40, 50, 60, 20, 04, 50, 14, 15, 10];

        public static byte[] GenerateKey(string password)
        {
            Rfc2898DeriveBytes rfc2898DeriveBytes = new(password, salt, 32, HashAlgorithmName.SHA256);
            return rfc2898DeriveBytes.GetBytes(32);
        }

        public static byte[] Encrypt(byte[] data, byte[] encryptionKey)
        {
            using MemoryStream final = new();
            using Aes aes = Aes.Create();

            final.Write(aes.IV, 0, aes.IV.Length);
            aes.Key = encryptionKey;

            using (CryptoStream cryptoStream = new(final, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
            }

            return final.ToArray();
        }

        public static byte[] Decrypt(byte[] encryptedData, byte[] encryptionKey)
        {
            using MemoryStream dataStream = new(encryptedData);
            using Aes aes = Aes.Create();

            dataStream.Read(aes.IV, 0, aes.IV.Length);
            aes.Key = encryptionKey;

            using MemoryStream final = new();
            using (CryptoStream cryptoStream = new(dataStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(final);
            }

            return final.ToArray();
        }
    }
}

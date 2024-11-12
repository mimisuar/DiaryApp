using System.Security.Cryptography;
using System.Text;

namespace Diary.Server.Services
{
    public class CryptoService
    {
        private static readonly byte[] salt = { 10, 3, 203, 0, 20, 20 };
        private readonly byte[] appKey;

        public CryptoService(IConfiguration config)
        {
            string key = config["Jwt:EncryptionKey"] ?? throw new InvalidOperationException("EncryptionKey is not set.");
            appKey = Encoding.ASCII.GetBytes(key);
        }

		private byte[] GenerateKeyFromPassword(string password, int keySize = 32)
        {
            Aes aes = Aes.Create();
   
            const int iters = 300;
            Rfc2898DeriveBytes keyGen = new Rfc2898DeriveBytes(password, salt, iters, HashAlgorithmName.SHA256);

            return keyGen.GetBytes(keySize);
        }

        public byte[] GenerateUserKey(string password)
        {
            using Aes aes = Aes.Create();
            byte[] passwordKey = GenerateKeyFromPassword(password);
            return EncryptText(Encoding.ASCII.GetString(aes.Key), passwordKey);
        }

        public string DecryptUserKey(byte[] encryptedUserKey, string password)
        {
            byte[] passwordKey = GenerateKeyFromPassword(password);
            return DecryptText(encryptedUserKey, passwordKey);
        }

        public byte[] EncryptText(string text, byte[] encryptionKey)
        {

            using MemoryStream stream = new();

            using Aes aes = Aes.Create();
			byte[] iv = aes.IV;
			byte[] key = encryptionKey;
			aes.Key = key;

            stream.Write(iv, 0, iv.Length);

            using CryptoStream crypto = new(stream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] rawData = Encoding.ASCII.GetBytes(text);
            crypto.Write(rawData, 0, rawData.Length);
            crypto.FlushFinalBlock();

			return stream.ToArray();
        }

        public byte[] EncryptText(string text)
        {
            return EncryptText(text, appKey);
        }

        public string DecryptText(byte[] encryptedText, byte[] encryptionKey)
        {
            using MemoryStream stream = new(encryptedText);
            using Aes aes = Aes.Create();

			byte[] iv = new byte[aes.IV.Length];
			byte[] key = encryptionKey;

            stream.Read(iv, 0, iv.Length);

			aes.Key = key;
			aes.IV = iv;

            using CryptoStream crypto = new(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(crypto);

            return reader.ReadToEnd();
		}

        public string DecryptText(byte[] encryptedText)
        {
            return DecryptText(encryptedText, appKey);
        }
    }
}

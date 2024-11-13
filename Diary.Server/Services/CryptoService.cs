using Diary.Server.Utils;
using System.Security.Cryptography;
using System.Text;

namespace Diary.Server.Services
{
    public class CryptoService
    {
        private readonly byte[] appKey;

        public CryptoService(IConfiguration config)
        {
            string key = config["Jwt:EncryptionKey"] ?? throw new InvalidOperationException("EncryptionKey is not set.");
            appKey = Encoding.ASCII.GetBytes(key);
        }

        public string EncryptUserKeyForDatabase(string userPassword)
        {
            byte[] userPasswordKey = Crypto.GenerateKey(userPassword);
            byte[] userKey;
            using (Aes aes = Aes.Create())
            {
                userKey = aes.Key;
            }

            byte[] encryptedUserKey = Crypto.Encrypt(userKey, userPasswordKey);
            return Convert.ToBase64String(encryptedUserKey);
        }

        public byte[] DecryptUserKeyFromDatabase(string base64UserKey, string userPassword)
        {
            byte[] userPasswordKey = Crypto.GenerateKey(userPassword);
            byte[] decodedUserKey = Convert.FromBase64String(base64UserKey);

            return Crypto.Decrypt(decodedUserKey, userPasswordKey);
        }

        public string EncryptUserKeyForClient(byte[] userKey)
        {
            return Convert.ToBase64String(Crypto.Encrypt(userKey, appKey));
        }

        public byte[] DecryptUserKeyFromClient(string base64UserKey)
        {
            return Crypto.Decrypt(Convert.FromBase64String(base64UserKey), appKey);
        }

        public string EncryptJournalBody(string body, byte[] userKey)
        {
            byte[] rawBody = Encoding.UTF8.GetBytes(body);
            byte[] encrypted = Crypto.Encrypt(rawBody, userKey);
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptJournalBody(string base64Body, byte[] userKey)
        {
            byte[] decoded = Convert.FromBase64String(base64Body);
            byte[] decrypted = Crypto.Decrypt(decoded, userKey);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}

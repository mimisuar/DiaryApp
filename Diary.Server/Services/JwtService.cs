using System.Text;

namespace Diary.Server.Services
{
    public class JwtService
    {
        private readonly byte[] privateKey;
        private readonly byte[] encryptionKey;
        public JwtService(IConfiguration configuration)
        {
            privateKey = Encoding.UTF8.GetBytes(configuration["Jwt:PrivateSigningKey"] ?? throw new InvalidOperationException("Jwt:PrivateSigningKey missing."));
            encryptionKey = Encoding.UTF8.GetBytes(configuration["Jwt:EncryptionKey"] ?? throw new InvalidOperationException("Jwt:EncryptionKey missing."));
        }

        
    }
}

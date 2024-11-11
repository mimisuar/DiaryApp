using Diary.Server.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Diary.Server.Services
{
    public class JwtService
    {

        private readonly string privateKey;
        private readonly string publicKey;
        private readonly string encryptionKey;
        private readonly ILogger<JwtService> logger;
        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            privateKey = configuration["Jwt:PrivateSigningKey"] ?? throw new InvalidOperationException("Jwt:PrivateSigningKey missing.");
            publicKey = configuration["Jwt:PublicSigningKey"] ?? throw new InvalidOperationException("Jwt:PublicSigningKey missing.");
            encryptionKey = configuration["Jwt:EncryptionKey"] ?? throw new InvalidOperationException("Jwt:EncryptionKey missing.");
            this.logger = logger;
        }

        public string GenerateToken(User user, string userKey)
        {
            JwtSecurityTokenHandler handler = new();

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            RsaSecurityKey rsaSecurityKey = new(rsa);
            SigningCredentials credentials = new(
                rsaSecurityKey,
                SecurityAlgorithms.RsaSha256
                );

            SymmetricSecurityKey symmetricKey = new(Encoding.ASCII.GetBytes(encryptionKey));
            EncryptingCredentials encryptingCredentials = new(symmetricKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);

            ClaimsIdentity claims = new();
            claims.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            claims.AddClaim(new Claim("Id", user.Id));
            claims.AddClaim(new Claim("Key", userKey));

            SecurityTokenDescriptor descriptor = new()
            {
                Subject = claims,
                Audience = "all",
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credentials,
                EncryptingCredentials = encryptingCredentials
            };

            return handler.CreateEncodedJwt(descriptor);
        }
    }
}

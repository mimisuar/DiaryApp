using Microsoft.AspNetCore.Identity;
using Diary.Server.Data;
using Microsoft.Extensions.Logging;
using Azure;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Diary.Server.Services
{
    public struct UserLoginResult
    {
        public bool Success { get; set; }
        public string? EncryptedUserKey { get; set; }
        public string? JsonWebToken { get; set; }
    }

    public class UserService
    {
        private readonly SignInManager<User> signInManager;
        private readonly CryptoService cryptoService;
        private readonly JwtService jwtService;
        private readonly ILogger<UserService> logger;
        public UserService(
            SignInManager<User> signInManager, 
            CryptoService cryptoService, 
            JwtService jwtService, 
            ILogger<UserService> logger
            )
        {
            this.signInManager = signInManager;
            this.cryptoService = cryptoService;
            this.jwtService = jwtService;
            this.logger = logger;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await signInManager.UserManager.FindByNameAsync(username);
        }

        public async Task<bool> RegisterUser(string username, string password)
        {
            User user = new()
            {
                UserName = username,
                EncryptedKey = cryptoService.GenerateUserKey(password)
            };

            IdentityResult result = await signInManager.UserManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to creater user.");
                foreach (IdentityError error in result.Errors)
                {
                    logger.LogError(error.Description);
                }
                return false;
            }

            return true;
        }

        public async Task<UserLoginResult> LoginUser(string username, string password)
        {
            User? user = await signInManager.UserManager.FindByNameAsync(username);
            if (user == null)
            {
                logger.LogError($"Failed to find user with name {username}");
                return new() { Success = false };
            }

            var result = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result == null || !result.Succeeded)
            {
                logger.LogError("Failed to sign user in.");
                return new() { Success = false };
            }

            string key = cryptoService.DecryptUserKey(user.EncryptedKey, password);
            return new()
            {
                Success = true,
                EncryptedUserKey = Encoding.ASCII.GetString(cryptoService.EncryptText(key)),
                JsonWebToken = jwtService.GenerateToken(user)
            };
        }

        public async Task LogoutUser()
        {
            await signInManager.SignOutAsync();
        }
    }
}

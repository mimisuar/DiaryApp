using Diary.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diary.Server.Services;
using System.Text;

namespace Diary.Server.Controllers
{
    public struct RegistrationFormData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public struct LoginFormData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly CryptoService cryptoService;
        private readonly ILogger<UserController> logger;

        public UserController(
            SignInManager<User> signInManager, 
            CryptoService cryptoService,
            ILogger<UserController> logger
            )
        {
            this.signInManager = signInManager;
            this.cryptoService = cryptoService;
            this.logger = logger;
        }

        [HttpPost("register")]
        public async Task RegisterUser([FromBody] RegistrationFormData registrationForm)
        {
            User user = new()
            {
                UserName = registrationForm.Username,
                EncryptedKey = cryptoService.GenerateUserKey(registrationForm.Password)
            };

            IdentityResult result = await signInManager.UserManager.CreateAsync(user, registrationForm.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to creater user.");
                foreach (IdentityError error in result.Errors)
                {
                    logger.LogError(error.Description);
                }
                return;
            }
        }

        [HttpPost("login")]
        public async Task LoginUser([FromBody] LoginFormData loginForm)
        {
            User? user = await signInManager.UserManager.FindByNameAsync(loginForm.Username);
            if (user == null)
            {
                logger.LogError($"Failed to find user with name {loginForm.Username}");
                return;
            }

            var result = await signInManager.PasswordSignInAsync(user, loginForm.Password, isPersistent: false, lockoutOnFailure: false);
            if (result == null || !result.Succeeded)
            {
                logger.LogError("Failed to sign user in.");
                return;
            }

            string key = cryptoService.DecryptUserKey(user.EncryptedKey, loginForm.Password);

            Response.Cookies.Append("enckey", key, new() { HttpOnly = true });
        }
    }
}

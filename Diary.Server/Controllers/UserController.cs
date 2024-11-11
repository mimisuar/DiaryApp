using Diary.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diary.Server.Services;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

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
        private readonly JwtService jwt;

        public UserController(
            SignInManager<User> signInManager, 
            CryptoService cryptoService,
            ILogger<UserController> logger,
            JwtService jwt
            )
        {
            this.signInManager = signInManager;
            this.cryptoService = cryptoService;
            this.logger = logger;
            this.jwt = jwt;
        }

        [HttpGet("testtoken")]
        [Authorize]
        public async Task TestToken()
        {
            string? id = await jwt.GetUserIdFromCurrentToken(HttpContext);
            if (id == null)
            {
                Response.StatusCode = 401;
                return;
            }

            await Response.WriteAsync($"UserId = ${id}");
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
                Response.StatusCode = 401;
                return;
            }

            var result = await signInManager.PasswordSignInAsync(user, loginForm.Password, isPersistent: false, lockoutOnFailure: false);
            if (result == null || !result.Succeeded)
            {
                logger.LogError("Failed to sign user in.");
                Response.StatusCode = 401;
                return;
            }

            string key = cryptoService.DecryptUserKey(user.EncryptedKey, loginForm.Password);
            await Response.WriteAsync(jwt.GenerateToken(user, key));
            
        }
    }
}

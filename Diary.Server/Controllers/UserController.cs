using Diary.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diary.Server.Services;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Security.Claims;

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
        private readonly UserService userService;

        public UserController(
            UserService userService
            )
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        public async Task RegisterUser([FromBody] RegistrationFormData registrationForm)
        {
            if (!await userService.RegisterUser(registrationForm.Username, registrationForm.Password))
            {
                Response.StatusCode = 401;
                return;
            }
        }

        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task LoginUser([FromBody] LoginFormData loginForm)
        {
            UserLoginResult result = await userService.LoginUser(loginForm.Username, loginForm.Password);

            if (!result.Success)
            {
                Response.StatusCode = 401;
                return;
            }

            Response.Cookies.Append("UserKey", result.EncryptedUserKey!);
            Response.Cookies.Append("JWT", result.JsonWebToken!);
            Response.Cookies.Append("Username", loginForm.Username);
        }

        [HttpPost("logout")]
        public async Task LogoutUser()
        {
            await userService.LogoutUser();
            Response.Cookies.Delete("UserKey");
            Response.Cookies.Delete("JWT");
            Response.Cookies.Delete("Username");
        }
    }
}

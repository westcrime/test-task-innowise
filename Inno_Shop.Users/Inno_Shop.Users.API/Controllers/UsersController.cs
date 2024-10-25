using Inno_Shop.Users.API.Middlewares;
using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Email;
using Inno_Shop.Users.Application.Services.Hash;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop.Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;

        public UsersController(IUserService userService, IEmailService emailService, ITokenService tokenService, IHashService hashService)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenService = tokenService;
            _hashService = hashService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerUserDto);

                Response.Cookies.Append("jwt", result, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true
                });

                return Ok(new
                {
                    Message = "Registration is successful!"
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var result = await _userService.LoginUserAsync(loginUserDto);

                Response.Cookies.Append("jwt", result, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true
                });

                return Ok(new
                {
                    Message = "Login is successful!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("validate")]
        public async Task<IActionResult> Validate()
        {
            return Ok(new
            {
                Success = true
            });
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] string email)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("jwt", out var token))
                {
                    return Unauthorized(new { message = "Token not found in cookies." });
                }

                var user = await _userService.GetUserByEmailAsync((await _tokenService.GetJwtPayload(token)).Email);

                if (user.Email != email && user.Role != Roles.Admin)
                {
                    return BadRequest(new { message = "Wrong role for this action." });
                }

                await _userService.DeleteUserAsync((await _userService.GetUserByEmailAsync(email)).Id);

                return Ok(new { message = "Successfully deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCodeForVerification()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("jwt", out var token))
                {
                    return Unauthorized(new { message = "Token not found in cookies." });
                }

                var user = await _userService.GetUserByEmailAsync((await _tokenService.GetJwtPayload(token)).Email);

                var code = await _emailService.SendEmailToken(user.Email);

                user.EmailToken = code;

                await _userService.UpdateUserAsync(user);

                return Ok(new { message = "Email code has been sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyUser([FromBody] string code)
        {
            try
            {
                if (!Request.Cookies.TryGetValue("jwt", out var token))
                {
                    return Unauthorized(new { message = "Token not found in cookies." });
                }

                var user = await _userService.GetUserByEmailAsync((await _tokenService.GetJwtPayload(token)).Email);

                if (code == user.EmailToken)
                {
                    user.IsVerified = true;

                    await _userService.UpdateUserAsync(user);
                }
                else
                {
                    throw new Exception("Wrong verification code!");
                }

                return Ok(new { message = "Your profile has been verified successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

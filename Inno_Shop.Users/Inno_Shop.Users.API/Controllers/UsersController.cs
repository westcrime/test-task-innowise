using Inno_Shop.Users.API.DTOs;
using Inno_Shop.Users.API.Extensions;
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            var resultResponse = await _userService.RegisterUserAsync(registerUserDto);

            if (resultResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(resultResponse.Result);
            }

            Response.Cookies.Append("jwt", resultResponse.Data, new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            });

            return Ok(new
            {
                Message = "Registration is successful!"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            var resultResponse = await _userService.LoginUserAsync(loginUserDto);

            if (resultResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(resultResponse.Result);
            }

            Response.Cookies.Append("jwt", resultResponse.Data, new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            });

            return Ok(new
            {
                Message = "Login is successful!"
            });
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

        [Authorize(Roles = "Admin")]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser([FromBody] string email)
        {
            var userResponse = await _userService.GetUserByEmailAsync(email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            return Ok(new { user = userResponse.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var userResponse = await _userService.GetAllUsersAsync();

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            return Ok(new { users = userResponse.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] string email)
        {
            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            var deleteResponse = await _userService.DeleteUserAsync(userResponse.Data.Id);

            if (deleteResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(deleteResponse.Result);
            }

            return Ok(new { message = "Successfully deleted." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto addUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var hashedPassword = await _hashService.GetHashAsync(addUserDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = addUserDto.Email,
                Password = hashedPassword,
                Role = Enum.Parse<Roles>(addUserDto.Role),
                Name = addUserDto.Name,
                IsVerified = addUserDto.IsVerified
            };

            var addResponse = await _userService.AddUserAsync(user);

            if (addResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(addResponse.Result);
            }

            return Ok(new { message = "Successfully added." });
        }


        [Authorize]
        [HttpPost("delete-my-account")]
        public async Task<IActionResult> DeleteUsersAccount()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            var deleteResponse = await _userService.DeleteUserAsync(userResponse.Data.Id);

            if (deleteResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(deleteResponse.Result);
            }

            return Ok(new { message = "Successfully deleted." });
        }

        [Authorize]
        [HttpPost("update-my-account")]
        public async Task<IActionResult> UpdateUsersAccount([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            if (updateUserDto.Email != null)
            {
                userResponse.Data.Email = updateUserDto.Email;
            }
            if (updateUserDto.Password != null)
            {
                userResponse.Data.Password = await _hashService.GetHashAsync(updateUserDto.Password);
            }
            if (updateUserDto.Name != null)
            {
                userResponse.Data.Name = updateUserDto.Name;
            }

            var updateResponse = await _userService.UpdateUserAsync(userResponse.Data);

            if (updateResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(updateResponse.Result);
            }

            return Ok(new { message = "Successfully updated." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateUserForAdminDto updateUserForAdminDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(updateUserForAdminDto.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            if (updateUserForAdminDto.Email != null)
            {
                userResponse.Data.Email = updateUserForAdminDto.Email;
            }
            if (updateUserForAdminDto.Password != null)
            {
                userResponse.Data.Password = await _hashService.GetHashAsync(updateUserForAdminDto.Password);
            }
            if (updateUserForAdminDto.Name != null)
            {
                userResponse.Data.Name = updateUserForAdminDto.Name;
            }
            if (updateUserForAdminDto.IsVerified != null)
            {
                userResponse.Data.IsVerified = (bool)updateUserForAdminDto.IsVerified;
            }
            if (updateUserForAdminDto.Name != null)
            {
                userResponse.Data.Name = updateUserForAdminDto.Name;
            }

            var updateResponse = await _userService.UpdateUserAsync(userResponse.Data);

            if (updateResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(updateResponse.Result);
            }

            return Ok(new { message = "Successfully updated." });
        }

        [Authorize]
        [HttpPost("send-code")]
        public async Task<IActionResult> SendCodeForVerification()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            var codeResponse = await _emailService.SendEmailToken(userResponse.Data.Email);

            if (codeResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(codeResponse.Result);
            }

            userResponse.Data.EmailToken = codeResponse.Data;

            var updateResponse = await _userService.UpdateUserAsync(userResponse.Data);

            if (updateResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(updateResponse.Result);
            }

            return Ok(new { message = "Email code has been sent successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            if (forgotPasswordDto.EmailCode == userResponse.Data.EmailToken)
            {
                userResponse.Data.Password = await _hashService.GetHashAsync(forgotPasswordDto.NewPassword);

                userResponse.Data.EmailToken = string.Empty;

                var updateResponse = await _userService.UpdateUserAsync(userResponse.Data);

                if (updateResponse.Result.IsFailure)
                {
                    return ResultExtensions.ToProblemDetails(updateResponse.Result);
                }
            }
            else
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(new Error("EmailCode", "Wrong verification code!")));
            }

            return Ok(new { message = "Password was changed successfully." });
        }

        [Authorize]
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyUser([FromBody] string code)
        {
            if (!Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Unauthorized(new { message = "Token not found in cookies." });
            }

            var getJwtPayloadResponse = await _tokenService.GetJwtPayload(token);

            if (getJwtPayloadResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(getJwtPayloadResponse.Result);
            }

            var userResponse = await _userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            if (code == userResponse.Data.EmailToken)
            {
                userResponse.Data.EmailToken = string.Empty;
            }
            else
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(new Error("EmailCode", "Wrong verification code!")));
            }

            return Ok(new { message = "Your profile has been verified successfully." });
        }
    }
}
 
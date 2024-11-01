using FluentValidation;
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
        private readonly IValidator<GetUserDto> _getUserDtoValidator;
        private readonly IValidator<DeleteUserDto> _deleteUserDtoValidator;
        private ILogger<UsersController> _logger;

        public UsersController(IValidator<GetUserDto> getUserDtoValidator,
            IValidator<DeleteUserDto> deleteUserDtoValidator,
            ILogger<UsersController> logger,
            IUserService userService, IEmailService emailService, ITokenService tokenService, IHashService hashService)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenService = tokenService;
            _hashService = hashService;
            _getUserDtoValidator = getUserDtoValidator;
            _deleteUserDtoValidator = deleteUserDtoValidator;
            _logger = logger;
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

            return Ok(new
            {
                Message = "Registration is successful!",
                Token = resultResponse.Data
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
                return ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            var resultResponse = await _userService.LoginUserAsync(loginUserDto);
            if (resultResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(resultResponse.Result);
            }

            return Ok(new
            {
                Message = "Login is successful!",
                Token = resultResponse.Data
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
        [HttpGet("get-user/{email}")]
        public async Task<IActionResult> GetUser([FromRoute] string email)
        {
            var getUserDto = new GetUserDto(email);

            var validationResult = await _getUserDtoValidator.ValidateAsync(getUserDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultExtensions.ToProblemDetails(Result.Failure(new Error(errors.ToString())));
            }

            var userResponse = await _userService.GetUserByEmailAsync(getUserDto.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            return Ok(userResponse.Data);
        }

        [Authorize]
        [HttpGet("get-my-account")]
        public async Task<IActionResult> GetUsersAccount()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            _logger.LogDebug($"Token: {token}");

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

            return Ok(userResponse.Data);
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

            return Ok(userResponse.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{email}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string email)
        {
            var deleteUserDto = new DeleteUserDto(email);

            var validationResult = await _deleteUserDtoValidator.ValidateAsync(deleteUserDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultExtensions.ToProblemDetails(Result.Failure(new Error(errors.ToString())));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }
            var token = authHeader.ToString().Replace("Bearer ", "");

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
        [HttpDelete("delete-my-account")]
        public async Task<IActionResult> DeleteUsersAccount()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

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
        [HttpPut("update-my-account")]
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

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

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
        [HttpPut("update")]
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

            var userResponse = await _userService.GetUserByIdAsync(Guid.Parse(updateUserForAdminDto.Id));

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
            if (updateUserForAdminDto.Role != null)
            {
                userResponse.Data.Role = Enum.Parse<Roles>(updateUserForAdminDto.Role);
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

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCodeForVerification(SendCodeDto sendCodeDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            var userResponse = await _userService.GetUserByEmailAsync(sendCodeDto.Email);

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

            var userResponse = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(userResponse.Result);
            }

            if (userResponse.Data.EmailToken != string.Empty && forgotPasswordDto.EmailCode == userResponse.Data.EmailToken)
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
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

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

            if (userResponse.Data.EmailToken != string.Empty && code == userResponse.Data.EmailToken)
            {
                userResponse.Data.EmailToken = string.Empty;
                userResponse.Data.IsVerified = true;
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

            return Ok(new { message = "Your profile has been verified successfully." });
        }
    }
}
 
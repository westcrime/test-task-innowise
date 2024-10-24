using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Email;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Domain.Entities;
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

        public UsersController(IUserService userService, IEmailService emailService, ITokenService tokenService)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto registerUserDto)
        {
            var resultResponse = await _userService.RegisterAsync(registerUserDto);

            if (resultResponse.Success)
            {
                return Ok(resultResponse);
            }
            return BadRequest(resultResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfoDto loginUserDto)
        {
            var resultResponse = await _userService.LoginAsync(loginUserDto);

            if (resultResponse.Success)
            {
                return Ok(new { Token = resultResponse.Data });
            }
            return Unauthorized(resultResponse);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] string token)
        {
            var resultResponse = await _tokenService.ValidateTokenAsync(token);

            if (resultResponse.Success)
            {
                return Ok(resultResponse);
            }
            return Unauthorized(resultResponse);
        }

        public async Task<ResponseModel<bool>> CheckIfUserIsAdminOrItIsHisAccount(PayloadDto payload, string id)
        {
            if (payload.Role == Domain.Entities.Roles.Admin)
            {
                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = true,
                    ErrorMessage = string.Empty
                };
            }
            else
            {
                var userResponse = await _userService.GetByEmailAsync(payload.Email);

                if (!userResponse.Success)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        ErrorMessage = userResponse.ErrorMessage
                    };
                }
                else
                {
                    if (userResponse.Data.Id != Guid.Parse(id))
                    {
                        return new ResponseModel<bool>
                        {
                            Success = true,
                            Data = false,
                            ErrorMessage = string.Empty
                        };
                    }
                    else
                    {
                        return new ResponseModel<bool>
                        {
                            Success = true,
                            Data = true,
                            ErrorMessage = string.Empty
                        };
                    }
                }
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] string token, [FromBody] string id)
        {
            var validateResponse = await _tokenService.ValidateTokenAsync(token);

            if (!validateResponse.Success)
            {
                return BadRequest(validateResponse);
            }

            var payloadResponse = await _tokenService.GetPayloadAsync(token);

            if (!payloadResponse.Success)
            {
                return BadRequest(payloadResponse);
            }

            var userToDeleteResponse = await _userService.GetByIdAsync(Guid.Parse(id));

            if (!userToDeleteResponse.Success)
            {
                return BadRequest(userToDeleteResponse);
            }

            var checkResponse = await CheckIfUserIsAdminOrItIsHisAccount(payloadResponse.Data, id);

            if (!checkResponse.Success)
            {
                return BadRequest(checkResponse);
            }

            if (checkResponse.Data == false)
            {
                return BadRequest(new ResponseModel<User>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "Can't do this with your role"
                });
            }
            else
            {
                return Ok(new ResponseModel<User>
                {
                    Success = true,
                    Data = userToDeleteResponse.Data,
                    ErrorMessage = string.Empty
                });
            }
        }
    }
}

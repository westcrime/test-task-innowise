using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Domain.Entities;
using Inno_Shop.Users.Application.Services.Token;

namespace Inno_Shop.Users.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepositry, ITokenService tokenService)
        {
            _userRepository = userRepositry;
            _tokenService = tokenService;
        }

        public async Task<ResponseModel<bool>> DeleteAccountAsync(Guid id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<ResponseModel<List<User>>> GetAllAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<ResponseModel<User>> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<ResponseModel<User>> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<ResponseModel<string>> LoginAsync(LoginInfoDto loginInfoDto)
        {
            var userResponse = await _userRepository.GetByEmailAsync(loginInfoDto.Email);

            if (!userResponse.Success)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = userResponse.ErrorMessage
                };
            }

            if (userResponse.Data.Password != loginInfoDto.Password)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    ErrorMessage = "Wrong password!",
                    Data = null
                };
            }

            var tokenResponse = await _tokenService.CreateTokenAsync(userResponse.Data);

            if (!tokenResponse.Success)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    ErrorMessage = tokenResponse.ErrorMessage,
                    Data = tokenResponse.Data
                };
            }

            return new ResponseModel<string>()
            {
                Success = true,
                ErrorMessage = string.Empty,
                Data = tokenResponse.Data
            };
        }

        public async Task<ResponseModel<string>> RegisterAsync(UserDto userDto)
        {
            var usersResponse = await _userRepository.GetAllUsersAsync();

            if (!usersResponse.Success)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = usersResponse.ErrorMessage
                };
            }

            if (usersResponse.Data.FirstOrDefault(user => user.Email == userDto.Email) != null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "User with this email already exists"
                };
            }

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = userDto.Email,
                Name = userDto.Name,
                Password = userDto.Password
            };

            var createUserResponse = await _userRepository.AddUserAsync(newUser);

            if (!createUserResponse.Success)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = createUserResponse.ErrorMessage
                };
            }

            var tokenResponse = await _tokenService.CreateTokenAsync(newUser);

            if (!tokenResponse.Success)
            {
                return new ResponseModel<string>()
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = tokenResponse.ErrorMessage
                };
            }

            return new ResponseModel<string>()
            {
                Success = true,
                Data = tokenResponse.Data,
                ErrorMessage = string.Empty
            };
        }

        public async Task<ResponseModel<bool>> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}

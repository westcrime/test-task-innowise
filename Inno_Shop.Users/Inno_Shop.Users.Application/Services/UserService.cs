using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Domain.Entities;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Application.Services.Hash;

namespace Inno_Shop.Users.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;

        public UserService(IUserRepository userRepositry, ITokenService tokenService, IHashService hashService)
        {
            _userRepository = userRepositry;
            _tokenService = tokenService;
            _hashService = hashService;
        }

        public async Task<Response<User>> AddUserAsync(User user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<Response<User>> DeleteUserAsync(Guid id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<Response<List<User>>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<Response<User>> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<Response<User>> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<Response<string>> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var userResponse = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

            if (userResponse.Result.IsFailure)
            {
                return new Response<string>(null, Result.Failure(UserErrors.WrongEmail));
            }

            var hashPasswordResponse = await _hashService.GetHashAsync(loginUserDto.Password);

            if (userResponse.Data.Password != hashPasswordResponse)
            {
                return new Response<string>(null, Result.Failure(UserErrors.WrongPassword));
            }

            var generateTokenResponse = await _tokenService.GenerateJwtTokenAsync(new JwtPayloadDto(userResponse.Data.Id, userResponse.Data.Email, userResponse.Data.Role));

            if (generateTokenResponse.Result.IsFailure)
            {
                return new Response<string>(null, generateTokenResponse.Result);
            }

            return new Response<string>(generateTokenResponse.Data, Result.Success());
        }

        public async Task<Response<string>> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var usersResponse = await _userRepository.GetAllUsersAsync();

            if (usersResponse.Result.IsFailure)
            {
                return new Response<string>(null, usersResponse.Result);
            }

            if (usersResponse.Data.FirstOrDefault(user => user.Email == registerUserDto.Email) != null)
            {
                return new Response<string>(null, Result.Failure(UserErrors.EmailAlreadyExists));
            }

            var hashPasswordResponse = await _hashService.GetHashAsync(registerUserDto.Password);

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = registerUserDto.Email,
                Name = registerUserDto.Name,
                Password = hashPasswordResponse
            };

            var result = await _userRepository.AddUserAsync(newUser);

            if (result.Result.IsFailure)
            {
                return new Response<string>(null, result.Result);
            }

            return await LoginUserAsync(new LoginUserDto(registerUserDto.Email, registerUserDto.Password));
        }

        public async Task<Response<User>> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }
    }

    public static class UserErrors
    {
        public static readonly Error EmailAlreadyExists = new("Users.EmailAlreadyExists", "User with this email already exists!");
        public static readonly Error NameAlreadyExists = new("Users.NameAlreadyExists", "User with this name already exists!");
        public static readonly Error WrongPassword = new("Users.WrongPassword", "Wrong password!");
        public static readonly Error WrongEmail = new("Users.WrongEmail", "Wrong email!");
    }
}

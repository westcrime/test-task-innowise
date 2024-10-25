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

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<string> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                throw new Exception("User is null!");
            }

            if (user.Password != (await _hashService.GetHashAsync(loginUserDto.Password)))
            {
                throw new Exception("Wrong password!");
            }

            return await _tokenService.GenerateJwtTokenAsync(new JwtPayloadDto(user.Id, user.Email));
        }

        public async Task<string> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (users.FirstOrDefault(user => user.Email == registerUserDto.Email) != null)
            {
                throw new Exception("User with this email already exists");
            }

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = registerUserDto.Email,
                Name = registerUserDto.Name,
                Password = await _hashService.GetHashAsync(registerUserDto.Password)
            };

            await _userRepository.AddUserAsync(newUser);

            return await LoginUserAsync(new LoginUserDto(registerUserDto.Email, registerUserDto.Password));
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }
    }
}

using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Hash;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Domain.Entities;
using Moq;
using Xunit;

namespace Inno_Shop.Users.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _hashServiceMock = new Mock<IHashService>();
            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object, _hashServiceMock.Object);
        }

        [Fact]
        public async Task AddUserAsync_ShouldReturnSuccess()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.AddUserAsync(user)).ReturnsAsync(new Response<User>(user, Result.Success()));

            // Act
            var result = await _userService.AddUserAsync(user);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(user.Email, result.Data.Email);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(userId)).ReturnsAsync(new Response<User>(user, Result.Success()));

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "test1@example.com" },
            new User { Id = Guid.NewGuid(), Email = "test2@example.com" }
        };
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(new Response<List<User>>(users, Result.Success()));

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(new Response<User>(user, Result.Success()));

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(email, result.Data.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new Response<User>(user, Result.Success()));

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginUserDto("test@example.com", "password123");
            var user = new User { Id = Guid.NewGuid(), Email = loginDto.Email, Password = "hashedPassword", Role = Roles.User };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(new Response<User>(user, Result.Success()));

            _hashServiceMock.Setup(service => service.GetHashAsync(loginDto.Password))
                .ReturnsAsync("hashedPassword");

            _tokenServiceMock.Setup(service => service.GenerateJwtTokenAsync(It.IsAny<JwtPayloadDto>()))
                .ReturnsAsync(new Response<string>("testToken", Result.Success()));

            // Act
            var result = await _userService.LoginUserAsync(loginDto);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal("testToken", result.Data);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterUserDto(Email: "test@example.com", Name: "Test User", Password: "password123");
            var user = new User { Id = Guid.NewGuid(), Email = registerDto.Email, Name = registerDto.Name, Password = "hashedPassword", Role = Roles.User };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(new Response<List<User>>(new List<User>(), Result.Success()));

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync("test@example.com"))
                .ReturnsAsync(new Response<User>(user, Result.Success()));

            _hashServiceMock.Setup(service => service.GetHashAsync(registerDto.Password))
                .ReturnsAsync("hashedPassword");

            _userRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync(new Response<User>(user, Result.Success()));

            _tokenServiceMock.Setup(service => service.GenerateJwtTokenAsync(It.IsAny<JwtPayloadDto>()))
                .ReturnsAsync(new Response<string>("testToken", Result.Success()));

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal("testToken", result.Data);
        }
    }
}
using Inno_Shop.Users.API.DTOs;
using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Inno_Shop.Users.API.IntegrationTests
{
    public class Tests
    {
        [Fact]
        public async Task Validate_UserNotLoggedIn_Forbidden()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();

            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Users/validate");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Login_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin@example.com", "adminpassword12Q!");
            var client = application.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync<LoginUserDto>("/api/Users/login", loginUserDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_Always_Failure()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin1@example.com", "adminpassword12Q!");
            var client = application.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync<LoginUserDto>("/api/Users/login", loginUserDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var registerUserDto = new RegisterUserDto("New User", "newuser@example.com", "NewUser12!");
            var client = application.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/Users/register", registerUserDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
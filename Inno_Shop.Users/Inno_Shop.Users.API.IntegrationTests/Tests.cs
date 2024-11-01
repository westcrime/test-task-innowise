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

        [Fact]
        public async Task AddTwoUsers_CheckTotalCount_Three()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin@example.com", "adminpassword12Q!");
            var user1 = new AddUserDto("newUser1", "example1@email.com", "blablaBLA12!!", true, "User");
            var user2 = new AddUserDto("newUser2", "example2@email.com", "blablaBLA12!!", true, "User");
            var client = application.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("/api/Users/login", loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JwtResponseDto>();
            var token = loginContent.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await client.PostAsJsonAsync<AddUserDto>("/api/Users/add", user1);
            await client.PostAsJsonAsync<AddUserDto>("/api/Users/add", user2);
            var getUsersResponse = await client.GetAsync("/api/Users/get-users");

            // Assert
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getUsersResponse.StatusCode);

            var users = await getUsersResponse.Content.ReadFromJsonAsync<List<User>>();
            Assert.Equal(3, users.Count); // ѕроверка, что количество пользователей равно 3
        }

        [Fact]
        public async Task UpdateUserProfile_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin@example.com", "adminpassword12Q!");
            var updateUserDto = new UpdateUserDto("Updated Admin", null);
            var client = application.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("/api/Users/login", loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JwtResponseDto>();
            var token = loginContent.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateResponse = await client.PutAsJsonAsync("/api/Users/update-my-account", updateUserDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedResource_WithoutLogin_BadRequest()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/Users/get/{"blalbla"}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddUserByAdminLogInDeleteGetInfo_CheckGetInfoStatus_BadRequest()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin@example.com", "adminpassword12Q!");
            var user1 = new AddUserDto("newUser1", "example1@email.com", "blablaBLA12!!", true, "User");
            var client = application.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("/api/Users/login", loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JwtResponseDto>();
            var token = loginContent.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await client.PostAsJsonAsync<AddUserDto>("/api/Users/add", user1);
            var newLoginResponse = await client.PostAsJsonAsync("/api/Users/login", new LoginUserDto("example1@email.com", "blablaBLA12!!"));
            newLoginResponse.EnsureSuccessStatusCode();
            var newLoginContent = await newLoginResponse.Content.ReadFromJsonAsync<JwtResponseDto>();
            var newToken = newLoginContent.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var validateResponse = await client.GetAsync("/api/Users/validate");
            validateResponse.EnsureSuccessStatusCode();
            var deleteResponse = await client.DeleteAsync("/api/Users/delete-my-account");
            deleteResponse.EnsureSuccessStatusCode();
            var getAccResponse = await client.GetAsync("/api/Users/get-my-account");

            // Assert
            Assert.Equal(HttpStatusCode.OK, newLoginResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, getAccResponse.StatusCode);
        }

        [Fact]
        public async Task CreateUserByAdminThenDeleteItThenTryeGetInfo_CheckGetInfoStatus_BadRequest()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = new LoginUserDto("admin@example.com", "adminpassword12Q!");
            var user1 = new AddUserDto("newUser1", "example1@email.com", "blablaBLA12!!", true, "User");
            var client = application.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("/api/Users/login", loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JwtResponseDto>();
            var token = loginContent.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await client.PostAsJsonAsync<AddUserDto>("/api/Users/add", user1);

            var deleteResponse = await client.DeleteAsync($"/api/Users/delete/{user1.Email}");
            deleteResponse.EnsureSuccessStatusCode();
            var getAccResponse = await client.GetAsync($"/api/Users/get{user1.Email}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getAccResponse.StatusCode);
        }
    }
}
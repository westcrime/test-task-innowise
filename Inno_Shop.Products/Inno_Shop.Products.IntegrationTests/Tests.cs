using System.Net.Http.Json;
using System.Net;
using Inno_Shop.Products.IntegrationTests.DTOs;
using System.Net.Http.Headers;
using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Domain.Entities;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using System.Collections.Generic;
using Inno_Shop.Products.IntegrationTests.Constants;

namespace Inno_Shop.Products.IntegrationTests
{
    public class Tests
    {
        [Fact]
        public async Task AddProduct_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = TestConstants.AdminCredentials();
            var addProductDto = new AddProductDto(
                "Test",
                "Test",
                1500,
                Guid.NewGuid());
            var client = application.CreateClient();


            var usersHttpClient = new HttpClient();
            var requestUri = TestConstants.LoginUri;

            // Act
            var loginResponse = await usersHttpClient.PostAsJsonAsync<LoginDto>(requestUri, loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>()).Token);

            var addResponse = await client.PostAsJsonAsync<AddProductDto>("/api/Products/add-product", addProductDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        }


        [Fact]
        public async Task AddProductGetProductUpdateProduct_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = TestConstants.AdminCredentials();

            var addProductDto = new AddProductDto(
                "Test",
                "Test",
                1500,
                null);

            var client = application.CreateClient();

            var usersHttpClient = new HttpClient();
            var requestUri = TestConstants.LoginUri;

            // Act
            var loginResponse = await usersHttpClient.PostAsJsonAsync(requestUri, loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>()).Token);

            var addResponse = await client.PostAsJsonAsync<AddProductDto>("/api/Products/add-product", addProductDto);
            addResponse.EnsureSuccessStatusCode();

            var getResponse = await client.GetAsync("/api/Products/get-my-products");
            getResponse.EnsureSuccessStatusCode();

            var product = (await getResponse.Content.ReadFromJsonAsync<List<Product>>())[0];

            var updateProductDto = new UpdateProductDto(
                product.Id,
                "Updated Test",
                "Updated Description",
                2000,
                null);

            var updateResponse = await client.PutAsJsonAsync("/api/Products/update-product", updateProductDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public async Task AddProductGetProductDeleteProduct_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = TestConstants.AdminCredentials();
            var client = application.CreateClient();

            var addProductDto = new AddProductDto(
                "Test",
                "Test",
                1500,
                null);

            var usersHttpClient = new HttpClient();
            var requestUri = TestConstants.LoginUri;

            // Act
            var loginResponse = await usersHttpClient.PostAsJsonAsync(requestUri, loginUserDto);
            loginResponse.EnsureSuccessStatusCode();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>()).Token);

            var addResponse = await client.PostAsJsonAsync<AddProductDto>("/api/Products/add-product", addProductDto);
            addResponse.EnsureSuccessStatusCode();

            var getResponse = await client.GetAsync("/api/Products/get-my-products");
            getResponse.EnsureSuccessStatusCode();

            var product = (await getResponse.Content.ReadFromJsonAsync<List<Product>>())[0];

            var deleteResponse = await client.DeleteAsync($"/api/Products/delete-product/{product.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllProducts_Always_Success()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var loginUserDto = TestConstants.AdminCredentials();
            var client = application.CreateClient();

            var usersHttpClient = new HttpClient();
            var requestUri = TestConstants.LoginUri;

            // Act
            var loginResponse = await usersHttpClient.PostAsJsonAsync(requestUri, loginUserDto);
            loginResponse.EnsureSuccessStatusCode();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>()).Token);

            var getResponse = await client.GetAsync("/api/Products/get-all-products");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetProductsNotLoggedIn_Always_Unauthorized()
        {
            // Arrange
            var application = new UsersWebApplicationFactory();
            var client = application.CreateClient();


            // Act
            var getResponse = await client.GetAsync("/api/Products/get-my-products");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }
    }
}
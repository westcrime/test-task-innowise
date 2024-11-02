using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Application.Repositories;
using Inno_Shop.Products.Application.Services;
using Inno_Shop.Products.Domain.Entities;
using Inno_Shop.Products.Infrastructure.Services.UserServices;
using Moq;
using Xunit;

namespace Inno_Shop.Products.UnitTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task AddProductAsync_ShouldReturnSuccess()
        {
            // Arrange
            var productDto = new AddProductDto("Bike", "Cool bike", 150, Guid.NewGuid());
            _productRepositoryMock.Setup(repo => repo.AddProductAsync(productDto)).ReturnsAsync(new Response<Product>(new Product()
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Cost = productDto.Cost,
                UserId = (Guid)productDto.UserId
            }, Result.Success()));

            // Act
            var result = await _productService.AddProductAsync(productDto);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(productDto.Name, result.Data.Name);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnSuccess()
        {
            // Arrange
            var productDto = new UpdateProductDto(Guid.NewGuid(), "Updated Bike", "Updated cool bike", 200, null);
            var productToUpdate = new Product
            {
                Id = productDto.Id,
                Name = "Old Bike",
                Description = "Old cool bike",
                Cost = 150,
                UserId = Guid.NewGuid()
            };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productDto.Id))
                .ReturnsAsync(new Response<Product>(productToUpdate, Result.Success()));

            _productRepositoryMock.Setup(repo => repo.UpdateProductAsync(productDto))
                .ReturnsAsync(new Response<Product>(new Product()
                {
                    Name = (string)(productDto.Name != null ? productDto.Name : productToUpdate.Name),
                    Description = (string)(productDto.Description != null ? productDto.Description : productToUpdate.Description),
                    Cost = (double)(productDto.Cost != null ? productDto.Cost : productToUpdate.Cost),
                    UserId = (Guid)(productDto.UserId != null ? productDto.UserId : productToUpdate.UserId)
                }, Result.Success()));

            // Act
            var result = await _productService.UpdateProductAsync(productDto);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal("Updated Bike", result.Data.Name);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnSuccess()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productToDelete = new Product
            {
                Id = productId,
                Name = "Old Bike",
                Description = "Old cool bike",
                Cost = 150,
                UserId = Guid.NewGuid()
            };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync(new Response<Product>(productToDelete, Result.Success()));

            _productRepositoryMock.Setup(repo => repo.DeleteProductAsync(productId))
                .ReturnsAsync(new Response<Product>(productToDelete, Result.Success()));

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(productId, result.Data.Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Bike 1", Description = "Cool bike 1", Cost = 150, UserId = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid(), Name = "Bike 2", Description = "Cool bike 2", Cost = 200, UserId = Guid.NewGuid() }
            };
            _productRepositoryMock.Setup(repo => repo.GetAllProductsAsync())
                .ReturnsAsync(new Response<List<Product>>(products, Result.Success()));

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Bike",
                Description = "Cool bike",
                Cost = 150,
                UserId = Guid.NewGuid()
            };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync(new Response<Product>(product, Result.Success()));

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(productId, result.Data.Id);
        }

        [Fact]
        public async Task GetAllUsersProductsAsync_ShouldReturnListOfUserProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Bike 1", Description = "Cool bike 1", Cost = 150, UserId = userId },
                new Product { Id = Guid.NewGuid(), Name = "Bike 2", Description = "Cool bike 2", Cost = 200, UserId = userId }
            };
            _productRepositoryMock.Setup(repo => repo.GetAllUsersProductsAsync(userId))
                .ReturnsAsync(new Response<List<Product>>(products, Result.Success()));

            // Act
            var result = await _productService.GetAllUsersProductsAsync(userId);

            // Assert
            Xunit.Assert.True(result.Result.IsSuccess);
            Xunit.Assert.Equal(2, result.Data.Count);
        }

    }
}
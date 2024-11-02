using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Application.Repositories;
using Inno_Shop.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<Product>> AddProductAsync(AddProductDto addProductDto)
        {
            return await _productRepository.AddProductAsync(addProductDto);
        }

        public async Task<Response<Product>> DeleteProductAsync(Guid id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<Response<Product>> DeleteUsersProductAsync(Guid id, Guid userId)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product.Result.IsFailure)
            {
                return product;
            }
            if (product.Data.UserId != userId)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<Response<List<Product>>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Response<List<Product>>> GetAllUsersProductsAsync(Guid userId)
        {
            return await _productRepository.GetAllUsersProductsAsync(userId);
        }

        public async Task<Response<Product>> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<Response<Product>> GetUsersProductByIdAsync(Guid id, Guid userId)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product.Result.IsFailure)
            {
                return product;
            }
            if (product.Data.UserId != userId)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<Response<Product>> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            return await _productRepository.UpdateProductAsync(updateProductDto);
        }

        public async Task<Response<Product>> UpdateUsersProductAsync(UpdateProductDto updateProductDto, Guid userId)
        {
            var product = await _productRepository.GetProductByIdAsync(updateProductDto.Id);
            if (product.Result.IsFailure)
            {
                return product;
            }
            if (product.Data.UserId != userId)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            return await _productRepository.UpdateProductAsync(updateProductDto);
        }
    }

    public static class ProductErrors
    {
        public static readonly Error EmailAlreadyExists = new("Products.GuidAlreadyExists", "Product with this id already exists!");
        public static readonly Error ProductDoesNotExist = new("Products.ProductDoesNotExist", "Product with this id does not exist!");
    }
}

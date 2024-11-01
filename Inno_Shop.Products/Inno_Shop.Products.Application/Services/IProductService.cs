using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Application.Services
{
    public interface IProductService
    {
        Task<Response<Product>> GetUsersProductByIdAsync(Guid id, Guid userId);
        Task<Response<List<Product>>> GetAllUsersProductsAsync(Guid userId);
        Task<Response<Product>> AddProductAsync(AddProductDto addProductDto);
        Task<Response<Product>> DeleteUsersProductAsync(Guid id, Guid userId);
        Task<Response<Product>> UpdateUsersProductAsync(UpdateProductDto updateProductDto, Guid userId);

        Task<Response<List<Product>>> GetAllProductsAsync();
        Task<Response<Product>> GetProductByIdAsync(Guid id);
        Task<Response<Product>> DeleteProductAsync(Guid id);
        Task<Response<Product>> UpdateProductAsync(UpdateProductDto updateProductDto);
    }
}

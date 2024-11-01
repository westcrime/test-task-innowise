using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Domain.Entities;

namespace Inno_Shop.Products.Application.Repositories
{
    public interface IProductRepository
    {
        Task<Response<Product>> GetProductByIdAsync(Guid id);
        Task<Response<List<Product>>> GetAllProductsAsync();
        Task<Response<List<Product>>> GetAllUsersProductsAsync(Guid userId);
        Task<Response<Product>> AddProductAsync(AddProductDto addProductDto);
        Task<Response<Product>> DeleteProductAsync(Guid id);
        Task<Response<Product>> UpdateProductAsync(UpdateProductDto updateProductDto);
    }
}

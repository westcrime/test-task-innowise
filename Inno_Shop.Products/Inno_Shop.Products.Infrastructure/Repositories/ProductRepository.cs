using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Application.Repositories;
using Inno_Shop.Products.Application.Services;
using Inno_Shop.Products.Domain.Entities;
using Inno_Shop.Products.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<Response<Product>> AddProductAsync(AddProductDto addProductDto)
        {
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Cost = addProductDto.Cost,
                UserId = (Guid)addProductDto.UserId
            };
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return new Response<Product>(newProduct, Result.Success());
        }

        public async Task<Response<Product>> DeleteProductAsync(Guid id)
        {
            var productToDelete = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productToDelete == null)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            else
            {
                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();
                return new Response<Product>(productToDelete, Result.Success());
            }
        }

        public async Task<Response<List<Product>>> GetAllProductsAsync()
        {
            return new Response<List<Product>>(await _context.Products.ToListAsync(), Result.Success());
        }

        public async Task<Response<List<Product>>> GetAllUsersProductsAsync(Guid userId)
        {
            var productsToFind = await _context.Products.Where(p => p.UserId ==  userId).ToListAsync();
            return new Response<List<Product>>(productsToFind, Result.Success());
        }

        public async Task<Response<Product>> GetProductByIdAsync(Guid id)
        {
            var productToFind = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productToFind == null)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            else
            {
                return new Response<Product>(productToFind, Result.Success());
            }
        }

        public async Task<Response<Product>> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.Id == updateProductDto.Id);
            if (productToUpdate == null)
            {
                return new Response<Product>(null, Result.Failure(ProductErrors.ProductDoesNotExist));
            }
            else
            {
                productToUpdate.Name = (string)(updateProductDto.Name != null ? updateProductDto.Name : productToUpdate.Name);
                productToUpdate.Description = (string)(updateProductDto.Description != null ? updateProductDto.Description : productToUpdate.Description);
                productToUpdate.Cost = (double)(updateProductDto.Cost != null ? updateProductDto.Cost : productToUpdate.Cost);
                productToUpdate.UserId = (Guid)(updateProductDto.UserId != null ? updateProductDto.UserId : productToUpdate.UserId);
                _context.Products.Update(productToUpdate);
                await _context.SaveChangesAsync();
                return new Response<Product>(productToUpdate, Result.Success());
            }
        }
    }
}

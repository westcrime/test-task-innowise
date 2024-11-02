using Inno_Shop.Products.API.Extensions;
using Inno_Shop.Products.Application.DTOs;
using Inno_Shop.Products.Application.Services;
using Inno_Shop.Products.Domain.Entities;
using Inno_Shop.Products.Infrastructure.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop.Products.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly UserService _userService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService, UserService userService)
        {
            _userService = userService;
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("get-my-products")]
        public async Task<IActionResult> GetUsersProducts()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            var productsResponse = await _productService.GetAllUsersProductsAsync(userResponse.Data.Id);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }
            
            return Ok(productsResponse.Data);
        }

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            if (userResponse.Data.Role != Roles.Admin)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden"
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var productsResponse = await _productService.GetAllProductsAsync();

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }
            
            return Ok(productsResponse.Data);
        }

        [HttpGet("get-my-product/{id}")]
        public async Task<IActionResult> GetUsersProduct([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var productGuid))
            {
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("Id.ValidationError", null)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            var productsResponse = await _productService.GetUsersProductByIdAsync(productGuid, userResponse.Data.Id);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok(productsResponse.Data);
        }

        [HttpGet("get-product/{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var productGuid))
            {
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("Id.ValidationError", null)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            if (userResponse.Data.Role != Roles.Admin)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden"
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var productsResponse = await _productService.GetProductByIdAsync(productGuid);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok(productsResponse.Data);
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto addProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            if (addProductDto.UserId == null)
            {
                addProductDto = new AddProductDto(addProductDto.Name, addProductDto.Description, addProductDto.Cost,
                    userResponse.Data.Id);
            }

            var productsResponse = await _productService.AddProductAsync(addProductDto);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok("The product was successfully added.");
        }

        [HttpPut("update-my-product")]
        public async Task<IActionResult> UpdateUsersProduct([FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            var productsResponse = await _productService.UpdateUsersProductAsync(updateProductDto, userResponse.Data.Id);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok("The product was successfully updated.");
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                var errorMessages = string.Join("; ", errors);
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("ValidationError", errorMessages)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            if (userResponse.Data.Role != Roles.Admin)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden"
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var productsResponse = await _productService.UpdateProductAsync(updateProductDto);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok("The product was successfully updated.");
        }

        [HttpDelete("delete-my-product/{id}")]
        public async Task<IActionResult> DeleteUsersProduct([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var productGuid))
            {
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("Id.ValidationError", null)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            var productsResponse = await _productService.DeleteUsersProductAsync(productGuid, userResponse.Data.Id);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok("The product was successfully deleted.");
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var productGuid))
            {
                ResultExtensions.ToProblemDetails(Result.Failure(new Error("Id.ValidationError", null)));
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized(new { message = "Authorization header not found." });
            }

            var token = authHeader.ToString().Replace("Bearer ", "");

            var userResponse = await _userService.GetCurrentUserAsync(token);

            if (userResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(userResponse.Result.Error));
            }

            if (userResponse.Data.Role != Roles.Admin)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden"
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var productsResponse = await _productService.DeleteProductAsync(productGuid);

            if (productsResponse.Result.IsFailure)
            {
                return ResultExtensions.ToProblemDetails(Result.Failure(productsResponse.Result.Error));
            }

            return Ok("The product was successfully deleted.");
        }
    }
}

using Inno_Shop.Products.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop.Products.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToProblemDetails(Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Can't convert successful request");
            }

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Extensions =
                    {
                        { "errors", new { result.Error } }
                    }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}

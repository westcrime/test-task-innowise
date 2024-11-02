using Inno_Shop.Users.API.Extensions;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Token;
using Microsoft.AspNetCore.Http;

namespace Inno_Shop.Users.API.Middlewares
{
    public class VerificationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IUserService userService, ITokenService tokenService)
        {
            try
            {
                var getJwtPayloadResponse = await tokenService.GetJwtPayload(context.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

                if (getJwtPayloadResponse.Result.IsFailure)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(getJwtPayloadResponse.Result.Error.Description);
                    return;
                }

                var userResponse = await userService.GetUserByEmailAsync(getJwtPayloadResponse.Data.Email);

                if (userResponse.Result.IsFailure)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(userResponse.Result.Error.Description);
                    return;
                }

                if (userResponse.Data == null || !userResponse.Data.IsVerified)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("User is not verified.");
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(ex.Message);
                return;
            }   
        }
    }
}

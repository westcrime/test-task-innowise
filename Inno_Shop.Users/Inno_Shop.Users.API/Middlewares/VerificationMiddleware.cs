using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Token;

namespace Inno_Shop.Users.API.Middlewares
{
    public class VerificationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IUserService userService, ITokenService tokenService)
        {
            try
            {
                var email = (await tokenService.GetJwtPayload(context.Request.Cookies["jwt"])).Email;

                var user = await userService.GetUserByEmailAsync(email);
                if (user == null || !user.IsVerified)
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

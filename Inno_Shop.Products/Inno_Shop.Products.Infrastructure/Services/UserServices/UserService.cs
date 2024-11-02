using Inno_Shop.Products.Domain.Entities;
using Inno_Shop.Products.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Inno_Shop.Products.Infrastructure.Services.UserServices
{
    public class UserService
    {
        private readonly IOptions<UserServerOptions> _userServerOptions;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IHttpClientFactory _clientFactory;

        public UserService(IOptions<UserServerOptions> userServerOptions, IOptions<JwtOptions> jwtOptions, IHttpClientFactory clientFactory)
        {
            _userServerOptions = userServerOptions;
            _jwtOptions = jwtOptions;
            _clientFactory = clientFactory;
        }

        public async Task<Response<User>> GetCurrentUserAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new Response<User>(null, Result.Failure(new Error("Token is null or empty.", null)));
            }

            var getAccountUri = $"{_userServerOptions.Value.Url}/api/Users/get-my-account";
            var request = new HttpRequestMessage(HttpMethod.Get, getAccountUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new Response<User>(null, Result.Failure(new Error(errorContent, null)));
            }

            var user = await response.Content.ReadFromJsonAsync<User>();
            return new Response<User>(user, Result.Success());
        }
    }
}

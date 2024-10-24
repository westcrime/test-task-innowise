using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly string _key = "1234567812345678123456781234567812345678";

    //public TokenService(string key)
    //{
    //    _key = key;
    //}

    public TokenService()
    {

    }

    public async Task<ResponseModel<string>> CreateTokenAsync(User user)
    {
        try
        {
            // Создание токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(10), // Время истечения
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new ResponseModel<string>
            {
                Success = true,
                Data = tokenString,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Data = null,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<ResponseModel<PayloadDto>> GetPayloadAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null)
            {
                return new ResponseModel<PayloadDto>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = "Invalid token"
                };
            }

            var payload = new PayloadDto(
                principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                (Roles)Int32.Parse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value)
            );

            return new ResponseModel<PayloadDto>
            {
                Success = true,
                Data = payload,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel<PayloadDto>
            {
                Success = false,
                Data = null,
                ErrorMessage = ex.Message
            };
        }
    }


    public async Task<ResponseModel<bool>> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return new ResponseModel<bool>
            {
                Success = true,
                Data = true,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Domain.Entities;
using Inno_Shop.Users.Infrastructure.JwtOptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public async Task<Response<string>> GenerateJwtTokenAsync(JwtPayloadDto jwtPayloadDto)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims = {
            new Claim(ClaimTypes.NameIdentifier, jwtPayloadDto.Id.ToString()),
            new Claim(ClaimTypes.Email, jwtPayloadDto.Email),
            new Claim(ClaimTypes.Role, jwtPayloadDto.Role.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddSeconds(_options.Seconds));

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new Response<string>(tokenValue, Result.Success());
    }

    public async Task<Response<JwtPayloadDto>> GetJwtPayload(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.SecretKey);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var payload = new JwtPayloadDto(
                Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value),
                principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Enum.Parse<Roles>(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value)
            );

            return new Response<JwtPayloadDto>(payload, Result.Success());
        }
        catch (Exception ex)
        {
            return new Response<JwtPayloadDto>(null, Result.Failure(new Error("TokenValidationError", ex.Message)));
        }
    }
}

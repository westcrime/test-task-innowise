using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.Services.Token
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(JwtPayloadDto jwtPayloadDto);
        Task<JwtPayloadDto> GetJwtPayload(string token);
    }
}

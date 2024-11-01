using Inno_Shop.Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Infrastructure.DTOs
{
    public record JwtPayloadDto(Guid Id, string Email, Roles Role);
}

﻿using Inno_Shop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.DTOs
{
    public record JwtPayloadDto(Guid Id, string Email, Roles Role);
}

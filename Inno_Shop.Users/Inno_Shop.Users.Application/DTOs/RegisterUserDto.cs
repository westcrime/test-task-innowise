﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.DTOs
{
    public record RegisterUserDto(string Name, string Email, string Password);
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Application.DTOs
{
    public record AddProductDto(string Name, string Description, double Cost, Guid UserId);
}

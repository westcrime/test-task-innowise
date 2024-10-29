using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Domain.Entities
{
    public sealed record Error(string Code, string? Description = null)
    {
        public static readonly Error None = new(string.Empty);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Infrastructure.Options
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int Seconds { get; set; }
    }
}

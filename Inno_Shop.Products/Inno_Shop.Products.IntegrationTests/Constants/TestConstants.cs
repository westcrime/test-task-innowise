using Inno_Shop.Products.IntegrationTests.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.IntegrationTests.Constants
{
    public static class TestConstants
    {
        public static LoginDto AdminCredentials() => new LoginDto("admin@example.com", "adminpassword12Q!");
        public static string LoginUri = "http://localhost:5098/api/Users/login";
    }
}

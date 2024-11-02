using Inno_Shop.Users.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.IntegrationTests.Constants
{
    public static class TestConstants
    {
        public static LoginUserDto AdminCredentials() => new LoginUserDto("admin@example.com", "adminpassword12Q!");
    }
}

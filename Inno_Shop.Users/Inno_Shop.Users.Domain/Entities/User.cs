using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; } = Roles.User;

        public string EmailToken { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
    }

    public enum Roles {
        Admin,
        User
    }
}

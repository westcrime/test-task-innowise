using Inno_Shop.Users.Application.Services.Hash;
using Inno_Shop.Users.Domain.Entities;
using Inno_Shop.Users.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Infrastructure.DbContext
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context, IHashService hashService, IOptions<AdminCredentialsOptions> options)  
        {
            context.Database.EnsureCreated();

            // Проверяем, если уже есть админ
            if (context.Users.Any(u => u.Role == Roles.Admin))
            {
                return; // Админ уже существует
            }

            // Создаем нового администратора
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = options.Value.Name,
                Email = options.Value.Email,
                Password = hashService.GetHash(options.Value.Password),
                Role = Roles.Admin,
                IsVerified = true,
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }

}

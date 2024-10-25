using Inno_Shop.Users.Application.DTOs;
using Inno_Shop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.Services
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(RegisterUserDto userDto);
        Task DeleteUserAsync(Guid id);
        Task<string> LoginUserAsync(LoginUserDto loginInfoDto);
        Task<User> GetUserByIdAsync(Guid id);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
    }
}

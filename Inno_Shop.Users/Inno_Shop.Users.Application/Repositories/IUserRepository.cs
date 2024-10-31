using Inno_Shop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.Repositories
{
    public interface IUserRepository
    {
        Task<Response<User>> GetUserByIdAsync(Guid id);
        Task<Response<User>> GetUserByEmailAsync(string email);
        Task<Response<List<User>>> GetAllUsersAsync();
        Task<Response<User>> AddUserAsync(User user);
        Task<Response<User>> DeleteUserAsync(Guid id);
        Task<Response<User>> UpdateUserAsync(User user);
    }
}

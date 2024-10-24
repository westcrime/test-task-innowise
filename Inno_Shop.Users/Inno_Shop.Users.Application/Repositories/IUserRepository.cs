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
        Task<ResponseModel<User>> GetByIdAsync(Guid id);
        Task<ResponseModel<User>> GetByEmailAsync(string email);
        Task<ResponseModel<List<User>>> GetAllUsersAsync();
        Task<ResponseModel<bool>> AddUserAsync(User user);
        Task<ResponseModel<bool>> DeleteUserAsync(Guid id);
        Task<ResponseModel<bool>> UpdateUserAsync(User user);
    }
}

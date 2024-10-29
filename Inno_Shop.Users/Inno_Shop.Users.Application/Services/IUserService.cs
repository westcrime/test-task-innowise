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
        Task<Response<string>> RegisterUserAsync(RegisterUserDto userDto);
        Task<Response<User>> DeleteUserAsync(Guid id);
        Task<Response<string>> LoginUserAsync(LoginUserDto loginInfoDto);
        Task<Response<User>> GetUserByIdAsync(Guid id);
        Task<Response<List<User>>> GetAllUsersAsync();
        Task<Response<User>> GetUserByEmailAsync(string email);
        Task<Response<User>> UpdateUserAsync(User user);
        Task<Response<User>> AddUserAsync(User user);
    }
}

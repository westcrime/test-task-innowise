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
        Task<ResponseModel<string>> RegisterAsync(UserDto userDto);
        Task<ResponseModel<bool>> DeleteAccountAsync(Guid id);
        Task<ResponseModel<string>> LoginAsync(LoginInfoDto loginInfoDto);
        Task<ResponseModel<User>> GetByIdAsync(Guid id);
        Task<ResponseModel<List<User>>> GetAllAsync();
        Task<ResponseModel<User>> GetByEmailAsync(string email);
        Task<ResponseModel<bool>> UpdateUserAsync(User user);
    }
}

using Inno_Shop.Users.Domain.Entities;

namespace Inno_Shop.Users.API.DTOs
{
    public record AddUserDto(string Name, string Email, string Password, bool IsVerified, string Role);
}

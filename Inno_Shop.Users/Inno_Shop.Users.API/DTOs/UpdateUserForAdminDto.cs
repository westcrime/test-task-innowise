namespace Inno_Shop.Users.API.DTOs
{
    public record UpdateUserForAdminDto(string? Email, string? Name, string? Password, bool? IsVerified, string Role);
}

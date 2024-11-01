﻿using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop.Users.API.DTOs
{
    public record ForgotPasswordDto(string Email, string EmailCode, string NewPassword);
}
using Inno_Shop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.Services.Email
{
    public interface IEmailService
    {
        Task<Response<string>> SendEmailToken(string email);
    }
}

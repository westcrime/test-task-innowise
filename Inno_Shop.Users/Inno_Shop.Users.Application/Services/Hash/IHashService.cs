using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Application.Services.Hash
{
    public interface IHashService
    {
        Task<string> GetHashAsync(string key);
    }
}

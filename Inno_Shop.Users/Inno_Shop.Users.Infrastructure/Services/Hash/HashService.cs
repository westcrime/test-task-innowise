using Inno_Shop.Users.Application.Services.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Infrastructure.Services.Hash
{
    public class HashService : IHashService
    {
        public async Task<string> GetHashAsync(string key)
        {
            return await Task.Run(() =>
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(key));

                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }

                    return builder.ToString();
                }
            });
        }
    }
}

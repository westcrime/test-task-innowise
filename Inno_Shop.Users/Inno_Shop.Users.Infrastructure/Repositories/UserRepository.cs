using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<User>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return new ResponseModel<User>
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }
        return new ResponseModel<User>
        {
            Success = true,
            Data = user
        };
    }

    public async Task<ResponseModel<User>> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return new ResponseModel<User>
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }
        return new ResponseModel<User>
        {
            Success = true,
            Data = user
        };
    }

    public async Task<ResponseModel<List<User>>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return new ResponseModel<List<User>>
        {
            Success = true,
            Data = users
        };
    }

    public async Task<ResponseModel<bool>> AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new ResponseModel<bool>
        {
            Success = true,
            Data = true
        };
    }

    public async Task<ResponseModel<bool>> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return new ResponseModel<bool>
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return new ResponseModel<bool>
        {
            Success = true,
            Data = true
        };
    }

    public async Task<ResponseModel<bool>> UpdateUserAsync(User user)
    {
        if ((await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id)) != null)
        {
            return new ResponseModel<bool>
            {
                Success = false,
                Data = false,
                ErrorMessage = "User with this email already Exists"
            };
        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return new ResponseModel<bool>
        {
            Success = true,
            Data = true
        };
    }
}

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

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"User with id {id} null exception!");
        }
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception($"User with email {email} null exception!");
        }
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"User with id {id} null exception!");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        if ((await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id)) != null)
        {
            throw new Exception("User with this email already Exists");
        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}

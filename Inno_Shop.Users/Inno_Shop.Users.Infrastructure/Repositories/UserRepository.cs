using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.Services;
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

    public async Task<Response<User>> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"User with id {id} null exception!");
        }
        return new Response<User>(user, Result.Success());
    }

    public async Task<Response<User>> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return new Response<User>(null, Result.Failure(UserErrors.WrongEmail));
        }
        return new Response<User>(user, Result.Success());
    }

    public async Task<Response<List<User>>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return new Response<List<User>>(users, Result.Success());
    }

    public async Task<Response<User>> AddUserAsync(User user)
    {
        if (await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email) != null)
        {
            return new Response<User>(null, Result.Failure(UserErrors.EmailAlreadyExists));
        }

        if (await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name) != null)
        {
            return new Response<User>(null, Result.Failure(UserErrors.NameAlreadyExists));
        }
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new Response<User>(user, Result.Success());
    }

    public async Task<Response<User>> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"User with id {id} null exception!");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return new Response<User>(user, Result.Success());
    }

    public async Task<Response<User>> UpdateUserAsync(User user)
    {
        if ((await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id)) != null)
        {
            return new Response<User>(null, Result.Failure(UserErrors.EmailAlreadyExists));
        }

        if ((await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name && u.Id != user.Id)) != null)
        {
            return new Response<User>(null, Result.Failure(UserErrors.NameAlreadyExists));
        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return new Response<User>(user, Result.Success());
    }
}
